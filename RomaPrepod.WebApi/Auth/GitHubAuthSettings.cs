namespace RomaPrepod.WebApi.Auth
{
	public class GitHubAuthSettings
	{
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string BaseUrl { get; set; }
		public string AccessTokenUrl { get; set; }
		public string CallbackUrl { get; set; }
		public string UserApiUrl { get; set; }
		public string UserAgent { get; set; }
	}
}
