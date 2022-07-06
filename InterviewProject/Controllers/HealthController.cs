using Microsoft.AspNetCore.Mvc;

namespace InterviewProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => this.Ok("It's working!");
    }
}
