using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMediatorService _mediatorService;

        public HomeController(ILogger<HomeController> logger,
            IMediatorService mediatorService)
        {
            _logger = logger;
            _mediatorService = mediatorService;
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            _logger.LogInformation("I am alive");
            return Ok("Alive");
        }

        [HttpGet("check")]
        public IActionResult Check()
        {
            //_mediatorService.ProcessDBResponse();
            _logger.LogInformation("I am alive");
            return Ok("Alive");
        }
    }
}
