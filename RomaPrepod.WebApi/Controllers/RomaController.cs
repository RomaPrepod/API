using Microsoft.AspNetCore.Mvc;

namespace RomaPrepod.WebApi.Controllers
{
	[Route("api/[controller]")]
	public class RomaController : Controller
	{
		[HttpGet]
		public Roma Get()
		{
			return new Roma
			{
				Name = "Рома",
				Occupation = "препод"
			};
		}
	}

	public class Roma
	{
		public string Name { get; set; }
		public string Occupation { get; set; }
	}
}
