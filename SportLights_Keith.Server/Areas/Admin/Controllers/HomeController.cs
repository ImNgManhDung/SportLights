using Microsoft.AspNetCore.Mvc;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Controllers
{
	[ApiController]
	[Route("api/admin/[controller]")]
	public class HomeController : ControllerBase
	{
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}


	}
}
