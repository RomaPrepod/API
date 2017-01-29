using System;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RomaPrepod.WebApi.Auth;

namespace RomaPrepod.WebApi
{
	public class Startup
	{
		private const string SecretKey = "needtogetthisfromenvironment";
		private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

		private const string IndexPath = "/index.html";

		private readonly string[] _angularRoutes =
		{
			"lectures",
			"labs",
			"tests",
			"results"
		};

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
			services.Configure<JwtIssuerOptions>(options =>
			{
				options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
				options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
				options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
				options.ValidFor = TimeSpan.FromMinutes(15);
			});
			services.Configure<GitHubAuthSettings>(Configuration.GetSection(nameof(GitHubAuthSettings)));
			services.AddTransient<GitHubAuthProvider>();

			services.AddMvc();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();


			IConfigurationSection jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

			app.UseJwtBearerAuthentication(new JwtBearerOptions
			{
				AutomaticAuthenticate = true,
				AutomaticChallenge = true,
				TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = _signingKey,
					ValidateIssuer = true,
					ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
					ValidateAudience = true,
					ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
					ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero
				}
			});

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.Use(async (context, next) =>
			{
				await next();

				if (context.Response.StatusCode == (int)HttpStatusCode.NotFound &&
					_angularRoutes.Any(route => context.Request.Path.Value.StartsWith("/"+route, StringComparison.OrdinalIgnoreCase)))
				{
					context.Request.Path = IndexPath;
					await next();
				}
			});

			app.UseDefaultFiles();
			app.UseStaticFiles();
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					"Login",
					"login/{action}",
					new {controller = "Login", action = "login"});
			});

		}
	}
}
