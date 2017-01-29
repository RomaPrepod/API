using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RomaPrepod.WebApi.Auth
{
	public class GitHubAuthProvider
	{
		private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
		{
			ContractResolver = new DefaultContractResolver
			{
				NamingStrategy = new SnakeCaseNamingStrategy()
			}
		};

		private readonly GitHubAuthSettings _settings;
		private readonly JwtIssuerOptions _jwtOptions;

		public GitHubAuthProvider(IOptions<GitHubAuthSettings> settings, IOptions<JwtIssuerOptions> jwtOptions)
		{
			ValidateAuthSettings(settings.Value);
			ValidateJwtOptions(jwtOptions.Value);

			_settings = settings.Value;
			_jwtOptions = jwtOptions.Value;
		}

		public string GetLoginUrl(string state = null)
		{
			string result =
				_settings.BaseUrl +
				"?client_id=" + _settings.ClientId +
				"&rediret_uri=" + _settings.CallbackUrl;

			if (string.IsNullOrWhiteSpace(state))
				return result;

			return result + "&state=" + state;
		}

		public async Task<string> GetTokenAsync(string code)
		{
			string accessToken = await GetAccessTokenAsync(code);
			UserData userData = await GetUserAsync(accessToken);
			List<Claim> claims = await GetClaimsAsync(userData);

			var jwt = new JwtSecurityToken(
				issuer: _jwtOptions.Issuer,
				audience: _jwtOptions.Audience,
				claims: claims,
				notBefore: _jwtOptions.NotBefore,
				expires: _jwtOptions.Expiration,
				signingCredentials: _jwtOptions.SigningCredentials);

			return new JwtSecurityTokenHandler().WriteToken(jwt);
		}

		private async Task<List<Claim>> GetClaimsAsync(UserData userData)
		{
			ClaimsIdentity identity = await GetClaimsIdentityAsync(userData);
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, userData.Login),
				new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
				new Claim(JwtRegisteredClaimNames.Iat,
					ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(),
					ClaimValueTypes.Integer64),
			};

			claims.AddRange(identity.FindAll("roles"));
			claims.AddRange(identity.FindAll("groups"));
			return claims;
		}

		private async Task<string> GetAccessTokenAsync(string code)
		{
			var request = WebRequest.CreateHttp(_settings.AccessTokenUrl);
			request.Method = "POST";
			request.ContentType = "application/json";
			request.Accept = "application/json";
			request.Headers[HttpRequestHeader.UserAgent] = _settings.UserAgent;

			await request.WriteJsonBodyAsync(new
			{
				_settings.ClientId,
				_settings.ClientSecret,
				code
			}, JsonSettings);

			var ghToken = await request.GetJsonResponseAsync<GitHubTokenResponse>(JsonSettings);
			return ghToken.AccessToken;
		}

		private async Task<UserData> GetUserAsync(string accessToken)
		{
			HttpWebRequest request = WebRequest.CreateHttp(_settings.UserApiUrl + "?access_token=" + accessToken);
			request.Method = "GET";
			request.Accept = "application/json";
			request.Headers[HttpRequestHeader.UserAgent] = _settings.UserAgent;

			GitHubUserResponse gitHubUser = await request.GetJsonResponseAsync<GitHubUserResponse>(JsonSettings);
			return await GetUserData(gitHubUser);
		}

		private static Task<UserData> GetUserData(GitHubUserResponse gitHubUser)
		{
			var result = new UserData
			{
				Login = gitHubUser.Login,
				Name = gitHubUser.Name,
				AvatarUrl = gitHubUser.AvatarUrl
			};


			// TODO: Implement Authorization
			if (gitHubUser.Login == "sergeykonkin")
			{
				result.Verified = true;
				result.VerifiedName = "Серёжа";
				result.Roles.Add("prepod");
				result.Roles.Add("admin");
			}

			return Task.FromResult(result);
		}

		private static async Task<ClaimsIdentity> GetClaimsIdentityAsync(UserData userData)
		{
			var rolesClaims = userData.Roles.Select(role => new Claim("roles", role));

			string name = userData.Verified
				? userData.VerifiedName
				: userData.Name;

			return await Task.FromResult(new ClaimsIdentity(
				new GenericIdentity(name, "Token"),
				rolesClaims));
		}

		/// <returns>
		/// Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).
		/// </returns>
		private static long ToUnixEpochDate(DateTime date)
			=> (long) Math.Round((date.ToUniversalTime() -
			                      new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
				.TotalSeconds);


		#region Validation

		private static void ValidateAuthSettings(GitHubAuthSettings settings)
		{
			if (settings == null) throw new ArgumentNullException(nameof(settings));
			if (string.IsNullOrWhiteSpace(settings.ClientId))
				throw new ArgumentException(nameof(GitHubAuthSettings.ClientId));
			if (string.IsNullOrWhiteSpace(settings.ClientSecret))
				throw new ArgumentException(nameof(GitHubAuthSettings.ClientSecret));
			if (string.IsNullOrWhiteSpace(settings.BaseUrl))
				throw new ArgumentException(nameof(GitHubAuthSettings.BaseUrl));
			if (string.IsNullOrWhiteSpace(settings.AccessTokenUrl))
				throw new ArgumentException(nameof(GitHubAuthSettings.AccessTokenUrl));
			if (string.IsNullOrWhiteSpace(settings.CallbackUrl))
				throw new ArgumentException(nameof(GitHubAuthSettings.CallbackUrl));
			if (string.IsNullOrWhiteSpace(settings.UserAgent))
				throw new ArgumentException(nameof(GitHubAuthSettings.UserAgent));
		}

		private static void ValidateJwtOptions(JwtIssuerOptions jwtOptions)
		{
			if (jwtOptions == null) throw new ArgumentNullException(nameof(jwtOptions));
			if (jwtOptions.ValidFor <= TimeSpan.Zero)
				throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
			if (jwtOptions.SigningCredentials == null)
				throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
			if (jwtOptions.JtiGenerator == null)
				throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
		}

		#endregion

		#region DTOs

		private class GitHubTokenResponse
		{
			public string AccessToken;
		}

		private class GitHubUserResponse
		{
			public string AvatarUrl;
			public string Name;
			public string Login;
		}

		private class UserData
		{
			public string AvatarUrl;
			public string Name;
			public string Login;
			public bool Verified;
			public string VerifiedName;
			public List<string> Groups;
			public List<string> Roles;

			public UserData()
			{
				Groups = new List<string>();
				Roles = new List<string>();
			}
		}

		#endregion
	}
}
