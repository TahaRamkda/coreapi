using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            _logger.LogInformation("Testing {logger} and name is {myname}", "axiom", "huzeifa");

            return Ok("Alive");
        }
    }
}
