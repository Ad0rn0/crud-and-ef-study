using Blog.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet("")]
        //[ApiKey]
        public IActionResult Get(
            [FromServices] IConfiguration config)
        {
            var environment = config.GetValue<string>("Environment");
            return Ok( new
            {
                environment = environment
            });
        }
    } 
}
