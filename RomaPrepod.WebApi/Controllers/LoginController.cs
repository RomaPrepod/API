using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RomaPrepod.WebApi.Auth;

namespace RomaPrepod.WebApi.Controllers
{
	public class LoginController : Controller
	{
		private readonly GitHubAuthProvider _gitHubAuthProvider;

		public LoginController(GitHubAuthProvider gitHubAuthProvider)
		{
			_gitHubAuthProvider = gitHubAuthProvider;
		}

		[HttpGet]
		public IActionResult Login()
		{
			return Redirect(_gitHubAuthProvider.GetLoginUrl());
		}

		[HttpGet]
		public async Task<IActionResult> Callback(string code, string state)
		{
			string token = await _gitHubAuthProvider.GetTokenAsync(code);
			return Content(token);
		}
	}
}
