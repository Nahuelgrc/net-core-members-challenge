using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using InterviewProject.Data;
using InterviewProject.Services;
using InterviewProject.Services.Abstractions;
using InterviewProject.Services.Models;
using InterviewProject.Shared;
using InterviewProject.Shared.Exceptions;
using InterviewProject.Tests.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InterviewProject.Tests.Services
{
    public class EmployeeServiceTests
    {
        public async Task<IMemberService<BusinessEmployee>> SetupService(MainDbContext mainDbContext = null)
        {
            if (mainDbContext == null)
            {
                mainDbContext = Utils.CreateDbContext();
            }

            await mainDbContext.SaveChangesAsync();

            return new EmployeeService(
                mainDbContext,
                Utils.GetMapper(),
                new Mock<ILogger<EmployeeService>>().Object);
        }

        [Fact]
        public async Task When_GetAll_IsCalled_Then_EmployeesAreReturned()
        {
            var mainDbContext = Utils.CreateDbContext(true);

            var member1 = FakeData.GetMember(1, JobType.Employee, "Test1");
            var member2 = FakeData.GetMember(2, JobType.Employee, "Test2");

            await mainDbContext.AddAsync(member1);
            await mainDbContext.AddAsync(member2);
            await mainDbContext.SaveChangesAsync();

            var membersTags1 = FakeData.GetMembersTags(1, member1.Id, 1);
            var membersTags2 = FakeData.GetMembersTags(2, member2.Id, 2);

            await mainDbContext.AddAsync(membersTags1);
            await mainDbContext.AddAsync(membersTags2);
            await mainDbContext.SaveChangesAsync();

            var newEmployee1 = FakeData.GetEmployee(1, EmployeeRoleType.ProjectManager, member1);
            var newEmployee2 = FakeData.GetEmployee(2, EmployeeRoleType.ScrumMaster, member2);

            await mainDbContext.AddAsync(newEmployee1);
            await mainDbContext.AddAsync(newEmployee2);
            await mainDbContext.SaveChangesAsync();

            var service = await this.SetupService(mainDbContext);

            var businessEmployeeResponse = await service.GetAll();

            var firstEmployee = businessEmployeeResponse.Where(x => x.Id == member1.Id)
                .SingleOrDefault();
            var secondEmployee = businessEmployeeResponse.Where(x => x.Id == member2.Id)
                .SingleOrDefault();

            Assert.Equal(2, businessEmployeeResponse.Count);
            Assert.Equal(newEmployee1.RoleType, firstEmployee.RoleType);
            Assert.Equal(member1.Name, firstEmployee.Name);
            Assert.Equal(member1.JobType, firstEmployee.JobType);
            Assert.Equal(1, firstEmployee.Tags.Count);
            Assert.Equal(newEmployee2.RoleType, secondEmployee.RoleType);
            Assert.Equal(member2.Name, secondEmployee.Name);
            Assert.Equal(member2.JobType, secondEmployee.JobType);
            Assert.Equal(1, secondEmployee.Tags.Count);
        }

        [Fact]
        public async Task When_GetById_IsCalled_Then_ExceptionIsThrown()
        {
            var service = await this.SetupService();

            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await service.GetById(1));
        }

        [Fact]
        public async Task When_GetById_IsCalled_Then_EmployeeIsReturned()
        {
            var mainDbContext = Utils.CreateDbContext(true);

            var member1 = FakeData.GetMember(1, JobType.Employee, "Test1");

            await mainDbContext.AddAsync(member1);
            await mainDbContext.SaveChangesAsync();

            var membersTags1 = FakeData.GetMembersTags(1, member1.Id, 1);

            await mainDbContext.AddAsync(membersTags1);
            await mainDbContext.SaveChangesAsync();

            var newEmployee1 = FakeData.GetEmployee(1, EmployeeRoleType.ProjectManager, member1);

            await mainDbContext.AddAsync(newEmployee1);
            await mainDbContext.SaveChangesAsync();

            var service = await this.SetupService(mainDbContext);

            var businessEmployeeResponse = await service.GetById(member1.Id);

            Assert.Equal(newEmployee1.RoleType, businessEmployeeResponse.RoleType);
            Assert.Equal(member1.Name, businessEmployeeResponse.Name);
            Assert.Equal(member1.JobType, businessEmployeeResponse.JobType);
            Assert.Equal(1, businessEmployeeResponse.Tags.Count);
        }

        [Fact]
        public async Task When_Create_IsCalled_Then_EmployeeIsCreatedInDatabase()
        {
            var newEmployee = Builder<BusinessEmployee>
                .CreateNew()
                .With(x => x.RoleType = EmployeeRoleType.ScrumMaster)
                .With(x => x.Name = "Test")
                .With(x => x.Tags = FakeData.GetBusinessTagList())
                .Build();

            var mainDbContext = Utils.CreateDbContext(true);

            var service = await this.SetupService(mainDbContext);

            var result = await service.Create(newEmployee);

            // Check response employee
            Assert.Equal(1, result.Id);
            Assert.Equal(newEmployee.RoleType, result.RoleType);
            Assert.Equal(newEmployee.Name, result.Name);
            Assert.Equal(newEmployee.Tags[0].Id, result.Tags[0].Id);
            Assert.Equal(newEmployee.Tags[1].Id, result.Tags[1].Id);

            // Check database employee
            var addedEmployee = mainDbContext.Employees
                .SingleOrDefault();

            Assert.Equal(newEmployee.RoleType, addedEmployee.RoleType);
            Assert.Equal(newEmployee.Name, addedEmployee.Member.Name);
            Assert.Equal(JobType.Employee, addedEmployee.Member.JobType);
            Assert.Equal(newEmployee.Tags.Count, addedEmployee.Member.MembersAndTags.Count);
        }

        [Fact]
        public async Task When_Update_IsCalled_Then_ExceptionIsThrown()
        {
            var service = await this.SetupService();

            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await service.Update(1, new BusinessEmployee()));
        }

        [Fact]
        public async Task When_Update_IsCalled_Then_EmployeeIsUpdated()
        {
            var mainDbContext = Utils.CreateDbContext(true);

            var member1 = FakeData.GetMember(1, JobType.Employee, "Test1");

            await mainDbContext.AddAsync(member1);
            await mainDbContext.SaveChangesAsync();

            var membersTags1 = FakeData.GetMembersTags(1, member1.Id, 1);

            await mainDbContext.AddAsync(membersTags1);
            await mainDbContext.SaveChangesAsync();

            var newEmployee1 = FakeData.GetEmployee(1, EmployeeRoleType.DeliveryManager, member1);

            await mainDbContext.AddAsync(newEmployee1);
            await mainDbContext.AddAsync(FakeData.GetTag(4));
            await mainDbContext.AddAsync(FakeData.GetTag(5));
            await mainDbContext.SaveChangesAsync();

            var service = await this.SetupService(mainDbContext);

            var newBusinessEmployee = new BusinessEmployee
            {
                RoleType = EmployeeRoleType.ScrumMaster,
                Name = "UpdateTest",
                Tags = FakeData.GetBusinessTagList(new int[] { 4, 5 }),
            };

            var result = await service.Update(member1.Id, newBusinessEmployee);

            // Check response employee
            Assert.Equal(1, result.Id);
            Assert.Equal(newBusinessEmployee.RoleType, result.RoleType);
            Assert.Equal(newBusinessEmployee.Name, result.Name);
            Assert.Equal(newBusinessEmployee.Tags[0].Id, result.Tags[0].Id);
            Assert.Equal(newBusinessEmployee.Tags[1].Id, result.Tags[1].Id);

            var employeeInDatabase = mainDbContext.Employees
                .Where(x => x.Id == member1.Id).SingleOrDefault();

            // Check database employee
            Assert.Equal(newEmployee1.RoleType, employeeInDatabase.RoleType);
            Assert.Equal(newEmployee1.Member.Name, employeeInDatabase.Member.Name);
            Assert.Equal(newEmployee1.Member.JobType, employeeInDatabase.Member.JobType);
            Assert.Equal(newEmployee1.Member.MembersAndTags.Count, employeeInDatabase.Member.MembersAndTags.Count);
        }

        [Fact]
        public async Task When_Delete_IsCalled_Then_ExceptionIsThrown()
        {
            var service = await this.SetupService();

            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await service.Delete(1));
        }

        [Fact]
        public async Task When_Delete_IsCalled_Then_EmployeeIsUpdated()
        {
            var mainDbContext = Utils.CreateDbContext(true);

            var member1 = FakeData.GetMember(1, JobType.Employee, "Test1");

            await mainDbContext.AddAsync(member1);
            await mainDbContext.SaveChangesAsync();

            var membersTags1 = FakeData.GetMembersTags(1, member1.Id, 1);

            await mainDbContext.AddAsync(membersTags1);
            await mainDbContext.SaveChangesAsync();

            var newEmployee1 = FakeData.GetEmployee(1, EmployeeRoleType.SoftwareEngineer, member1);
            await mainDbContext.AddAsync(newEmployee1);

            var service = await this.SetupService(mainDbContext);

            await service.Delete(member1.Id);

            var employeeInDatabase = mainDbContext.Employees
                .Where(x => x.Id == member1.Id).SingleOrDefault();

            Assert.Null(employeeInDatabase);
        }

        [Fact]
        public async Task When_Filter_IsCalled_Then_EmployeesAreReturned()
        {
            var mainDbContext = Utils.CreateDbContext(true);

            var member1 = FakeData.GetMember(1, JobType.Employee, "Test1");
            var member2 = FakeData.GetMember(2, JobType.Employee, "Test2");

            await mainDbContext.AddAsync(member1);
            await mainDbContext.AddAsync(member2);
            await mainDbContext.AddAsync(FakeData.GetTag(3));
            await mainDbContext.AddAsync(FakeData.GetTag(4));
            await mainDbContext.SaveChangesAsync();

            var membersTags1 = FakeData.GetMembersTags(1, member1.Id, 1);
            var membersTags2 = FakeData.GetMembersTags(2, member1.Id, 2);
            var membersTags3 = FakeData.GetMembersTags(3, member2.Id, 3);
            var membersTags4 = FakeData.GetMembersTags(4, member2.Id, 4);

            await mainDbContext.AddAsync(membersTags1);
            await mainDbContext.AddAsync(membersTags2);
            await mainDbContext.AddAsync(membersTags3);
            await mainDbContext.AddAsync(membersTags4);
            await mainDbContext.SaveChangesAsync();

            var newEmployee1 = FakeData.GetEmployee(1, EmployeeRoleType.DeliveryManager, member1);
            var newEmployee2 = FakeData.GetEmployee(2, EmployeeRoleType.ProjectManager, member2);

            await mainDbContext.AddAsync(newEmployee1);
            await mainDbContext.AddAsync(newEmployee2);
            await mainDbContext.SaveChangesAsync();

            var service = await this.SetupService(mainDbContext);

            var businessEmployeeResponse = await service.Filter(new int[] { 2, 4 });

            var firstBusinessEmployee = businessEmployeeResponse
                .Where(x => x.Id == member1.Id)
                .SingleOrDefault();

            Assert.Equal(newEmployee1.Member.Id, firstBusinessEmployee.Id);
            Assert.Equal(newEmployee1.Member.Name, firstBusinessEmployee.Name);
            Assert.Equal(newEmployee1.Member.MembersAndTags.Count, firstBusinessEmployee.Tags.Count);
            Assert.Equal(newEmployee1.RoleType, firstBusinessEmployee.RoleType);
            Assert.Equal(newEmployee1.Member.JobType, firstBusinessEmployee.JobType);

            var secondBusinessEmployee = businessEmployeeResponse
                .Where(x => x.Id == member2.Id)
                .SingleOrDefault();

            Assert.Equal(newEmployee2.Member.Id, secondBusinessEmployee.Id);
            Assert.Equal(newEmployee2.Member.Name, secondBusinessEmployee.Name);
            Assert.Equal(newEmployee2.Member.MembersAndTags.Count, secondBusinessEmployee.Tags.Count);
            Assert.Equal(newEmployee2.RoleType, secondBusinessEmployee.RoleType);
            Assert.Equal(newEmployee2.Member.JobType, secondBusinessEmployee.JobType);

            var firstEmployee = businessEmployeeResponse
                .Where(x => x.Id == member1.Id)
                .SingleOrDefault();

            Assert.Equal(newEmployee1.Member.Id, firstEmployee.Id);
            Assert.Equal(newEmployee1.RoleType, firstEmployee.RoleType);
            Assert.Equal(member1.Name, firstEmployee.Name);
            Assert.Equal(member1.JobType, firstEmployee.JobType);
            Assert.Equal(membersTags1.TagId, firstEmployee.Tags[0].Id);
            Assert.Equal(membersTags2.TagId, firstEmployee.Tags[1].Id);

            var secondEmployee = businessEmployeeResponse
                .Where(x => x.Id == member2.Id)
                .SingleOrDefault();

            Assert.Equal(newEmployee2.Member.Id, secondEmployee.Id);
            Assert.Equal(newEmployee2.RoleType, secondEmployee.RoleType);
            Assert.Equal(member2.Name, secondEmployee.Name);
            Assert.Equal(member2.JobType, secondEmployee.JobType);
            Assert.Equal(membersTags3.TagId, secondEmployee.Tags[0].Id);
            Assert.Equal(membersTags4.TagId, secondEmployee.Tags[1].Id);
        }
    }
}
