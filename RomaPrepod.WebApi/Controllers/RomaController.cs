using Microsoft.AspNetCore.Mvc;

namespace RomaPrepod.WebApi.Controllers
{
	[Route("api/[controller]")]
	public class RomaController : Controller
	{
		[HttpGet]
		public IActionResult Get()
		{
			return Ok(new { name = "Рома", occupation = "препод"});
		}
	}
}
