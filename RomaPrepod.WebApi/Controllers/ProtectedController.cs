using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace RomaPrepod.WebApi.Controllers
{
	[Authorize(ActiveAuthenticationSchemes = "Bearer")]
	[Route("api/[controller]")]
	public class ProtectedController : Controller
	{
		public string Get()
		{
			return "Secret data!";
		}
	}
}
