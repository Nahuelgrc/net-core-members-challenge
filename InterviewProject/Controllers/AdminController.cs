using InterviewProject.Controllers.Models.Requests;
using InterviewProject.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InterviewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService adminService;

        public AdminController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        [Route("tag")]
        [HttpPost]
        public async Task<ActionResult> CreateTag([FromBody] ApiAddTagRequest request)
        {
            await this.adminService.AddTag(request.Name);
            return this.Ok();
        }
    }
}
