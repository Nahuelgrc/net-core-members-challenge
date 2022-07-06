using System.Threading.Tasks;
using InterviewProject.Controllers;
using InterviewProject.Controllers.Models.Requests;
using InterviewProject.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InterviewProject.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<IAdminService> adminServiceMock;

        public AdminControllerTests()
        {
            this.adminServiceMock = new Mock<IAdminService>();
        }

        public AdminController SetupController()
        {
            return new AdminController(this.adminServiceMock.Object);
        }

        [Fact]
        public async Task When_CreateTag_IsCalled_Ok_IsReturned()
        {
            this.adminServiceMock.Setup(x => x.AddTag(It.IsAny<string>()));

            var controller = this.SetupController();

            var request = new ApiAddTagRequest
            {
                Name = "Tag1",
            };

            var result = await controller.CreateTag(request);
            var okResult = result as OkResult;

            Assert.IsAssignableFrom<ActionResult>(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}
