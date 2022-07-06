using System.Linq;
using System.Threading.Tasks;
using InterviewProject.Data;
using InterviewProject.Services;
using InterviewProject.Services.Abstractions;
using InterviewProject.Tests.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InterviewProject.Tests.Services
{
    public class AdminServiceTests
    {
        public IAdminService SetupService(MainDbContext mainDbContext = null)
        {
            if (mainDbContext == null)
            {
                mainDbContext = Utils.CreateDbContext();
            }

            return new AdminService(
                mainDbContext,
                new Mock<ILogger<AdminService>>().Object);
        }

        [Fact]
        public async Task When_AddTag_IsCalled_Then_TagIsCreatedInDatabase()
        {
            var newTag = "TestTag";
            var mainDbContext = Utils.CreateDbContext();
            var service = this.SetupService(mainDbContext);

            await service.AddTag(newTag);

            var addedTag = mainDbContext.Tags.SingleOrDefault();

            Assert.Equal(newTag, addedTag.Name);
        }
    }
}
