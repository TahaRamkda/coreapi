using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {

        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok("Alive");
        }
    }
}
