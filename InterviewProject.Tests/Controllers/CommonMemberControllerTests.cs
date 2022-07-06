using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using InterviewProject.Controllers;
using InterviewProject.Controllers.Models.Responses;
using InterviewProject.MappingConfiguration;
using InterviewProject.Services.Abstractions;
using InterviewProject.Services.Models;
using InterviewProject.Shared;
using InterviewProject.Tests.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InterviewProject.Tests.Controllers
{
    public class CommonMemberControllerTests
    {
        private readonly Mock<IMemberService<BusinessContractor>> contractorServiceMock;
        private readonly Mock<IMemberService<BusinessEmployee>> employeeServiceMock;

        public CommonMemberControllerTests()
        {
            this.contractorServiceMock = new Mock<IMemberService<BusinessContractor>>();
            this.employeeServiceMock = new Mock<IMemberService<BusinessEmployee>>();
        }

        public CommonMemberController SetupController()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<InterviewMappingProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            return new CommonMemberController(this.contractorServiceMock.Object, this.employeeServiceMock.Object, mapper);
        }

        [Fact]
        public async Task When_GetAll_Then_ContractorsAndEmployeesAreReturned()
        {
            var contractor1 = FakeData.GetBusinessContractor(FakeData.GetBusinessTagList());
            var contractor2 = FakeData.GetBusinessContractor(FakeData.GetBusinessTagList(), 2, 6, "Test2");
            var businessContractors = new List<BusinessContractor>()
            {
                contractor1,
                contractor2,
            };

            var employee1 = FakeData.GetBusinessEmployee(FakeData.GetBusinessTagList(), 3);
            var employee2 = FakeData.GetBusinessEmployee(FakeData.GetBusinessTagList(), 4, EmployeeRoleType.ProjectManager, "Test3");
            var businessEmployees = new List<BusinessEmployee>()
            {
                employee1,
                employee2,
            };

            this.contractorServiceMock.Setup(x => x.GetAll()).ReturnsAsync(businessContractors);
            this.employeeServiceMock.Setup(x => x.GetAll()).ReturnsAsync(businessEmployees);

            var controller = this.SetupController();

            var result = await controller.GetAll();
            var actual = (result.Result as OkObjectResult).Value as ApiGetAllResponse;

            Assert.Equal(businessContractors.Count + businessEmployees.Count, actual.Count);
            Assert.Equal(businessContractors.Count, actual.Contractors.Count);
            Assert.Equal(businessEmployees.Count, actual.Employees.Count);
        }

        [Fact]
        public async Task When_Search_Then_ContractorsAndEmployeesAreReturned()
        {
            var contractor1 = FakeData.GetBusinessContractor(FakeData.GetBusinessTagList());
            var contractor2 = FakeData.GetBusinessContractor(FakeData.GetBusinessTagList(), 2, 6, "Test2");
            var businessContractors = new List<BusinessContractor>()
            {
                contractor1,
                contractor2,
            };

            var employee1 = FakeData.GetBusinessEmployee(FakeData.GetBusinessTagList(), 3);
            var employee2 = FakeData.GetBusinessEmployee(FakeData.GetBusinessTagList(), 4, EmployeeRoleType.ProjectManager, "Test3");
            var businessEmployees = new List<BusinessEmployee>()
            {
                employee1,
                employee2,
            };

            this.contractorServiceMock.Setup(x => x.Filter(It.IsAny<IList<int>>())).ReturnsAsync(businessContractors);
            this.employeeServiceMock.Setup(x => x.Filter(It.IsAny<IList<int>>())).ReturnsAsync(businessEmployees);

            var controller = this.SetupController();

            var request = new List<int>
            {
                1,
            };

            var result = await controller.Search(request);
            var actual = (result.Result as OkObjectResult).Value as ApiGetAllResponse;

            Assert.Equal(businessContractors.Count + businessEmployees.Count, actual.Count);
            Assert.Equal(businessContractors.Count, actual.Contractors.Count);
            Assert.Equal(businessEmployees.Count, actual.Employees.Count);
        }
    }
}
