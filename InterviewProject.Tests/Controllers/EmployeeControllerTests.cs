using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FizzWare.NBuilder;
using InterviewProject.Controllers;
using InterviewProject.Controllers.Models.Requests;
using InterviewProject.Controllers.Models.Responses;
using InterviewProject.MappingConfiguration;
using InterviewProject.Services.Abstractions;
using InterviewProject.Services.Models;
using InterviewProject.Shared;
using InterviewProject.Shared.Exceptions;
using InterviewProject.Tests.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InterviewProject.Tests.Controllers
{
    public class EmployeeControllerTests
    {
        private readonly Mock<IMemberService<BusinessEmployee>> employeeServiceMock;

        public EmployeeControllerTests()
        {
            this.employeeServiceMock = new Mock<IMemberService<BusinessEmployee>>();
        }

        public EmployeeController SetupController()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<InterviewMappingProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            return new EmployeeController(this.employeeServiceMock.Object, mapper);
        }

        [Fact]
        public async Task When_GetAll_Then_EmployeesAreReturned()
        {
            var employee1 = FakeData.GetBusinessEmployee(FakeData.GetBusinessTagList());
            var employee2 = FakeData.GetBusinessEmployee(FakeData.GetBusinessTagList(), 2, EmployeeRoleType.ProjectManager, "Test2");
            var businessEmployees = new List<BusinessEmployee>()
            {
                employee1,
                employee2,
            };

            this.employeeServiceMock.Setup(x => x.GetAll()).ReturnsAsync(businessEmployees);

            var controller = this.SetupController();

            var result = await controller.GetAll();
            var actual = (result.Result as OkObjectResult).Value as ApiGetAllEmployeesResponse;

            Assert.Equal(businessEmployees.Count, actual.Count);
        }

        [Fact]
        public async Task When_GetById_Then_NotFoundIsReturned()
        {
            var employee = FakeData.GetBusinessEmployee(FakeData.GetBusinessTagList());

            this.employeeServiceMock
                .Setup(x => x.GetById(It.IsAny<int>()))
                .ThrowsAsync(new NotFoundException());

            var controller = this.SetupController();
            var result = await controller.GetById(employee.Id);
            var notFoundResult = result.Result as NotFoundResult;
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task When_GetById_Then_EmployeeIsReturned()
        {
            var employee = FakeData.GetBusinessEmployee(FakeData.GetBusinessTagList());

            this.employeeServiceMock
                .Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(employee);

            var controller = this.SetupController();
            var result = await controller.GetById(employee.Id);
            var actual = (result.Result as OkObjectResult).Value as ApiEmployeeResponse;

            Assert.Equal(employee.Id, actual.Id);
            Assert.Equal(employee.Name, actual.Name);
            Assert.Equal(employee.RoleType, actual.RoleType);
            Assert.Equal(employee.Tags.Count, actual.Tags.Count);
        }

        [Fact]
        public async Task When_Create_Then_EmployeeIsReturned()
        {
            var request = Builder<ApiEmployeeRequest>
                .CreateNew()
                .Build();

            var employee = FakeData.GetBusinessEmployee(FakeData.GetBusinessTagList());

            this.employeeServiceMock
                .Setup(x => x.Create(It.IsAny<BusinessEmployee>()))
                .ReturnsAsync(employee);

            var controller = this.SetupController();
            var result = await controller.Create(request);
            var actual = (result.Result as OkObjectResult).Value as ApiEmployeeResponse;

            Assert.Equal(employee.Id, actual.Id);
            Assert.Equal(employee.Name, actual.Name);
            Assert.Equal(employee.RoleType, actual.RoleType);
            Assert.Equal(employee.Tags.Count, actual.Tags.Count);
        }

        [Fact]
        public async Task When_Update_Then_NotFoundIsReturned()
        {
            var employee = FakeData.GetBusinessEmployee(FakeData.GetBusinessTagList());

            var request = Builder<ApiEmployeeUpdateRequest>
                .CreateNew()
                .Build();

            this.employeeServiceMock
                .Setup(x => x.Update(It.IsAny<int>(), It.IsAny<BusinessEmployee>()))
                .ThrowsAsync(new NotFoundException());

            var controller = this.SetupController();
            var result = await controller.Update(employee.Id, request);
            var notFoundResult = result.Result as NotFoundResult;
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task When_Update_Then_EmployeeIsUpdated()
        {
            var employee = FakeData.GetBusinessEmployee(FakeData.GetBusinessTagList());

            var request = Builder<ApiEmployeeUpdateRequest>
                .CreateNew()
                .Build();

            this.employeeServiceMock
                .Setup(x => x.Update(It.IsAny<int>(), It.IsAny<BusinessEmployee>()))
                .ReturnsAsync(employee);

            var controller = this.SetupController();
            var result = await controller.Update(employee.Id, request);
            var actual = (result.Result as OkObjectResult).Value as ApiEmployeeResponse;

            Assert.Equal(employee.Id, actual.Id);
            Assert.Equal(employee.Name, actual.Name);
            Assert.Equal(employee.RoleType, actual.RoleType);
            Assert.Equal(employee.Tags.Count, actual.Tags.Count);
        }

        [Fact]
        public async Task When_Delete_Then_NotFoundIsReturned()
        {
            this.employeeServiceMock
                .Setup(x => x.Delete(It.IsAny<int>()))
                .ThrowsAsync(new NotFoundException());

            var controller = this.SetupController();
            var result = await controller.Delete(1);
            var notFoundResult = result.Result as NotFoundResult;
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task When_Delete_Then_EmployeeIsUpdated()
        {
            this.employeeServiceMock
                .Setup(x => x.Delete(It.IsAny<int>()));

            var controller = this.SetupController();
            var result = await controller.Delete(1);
            var noContentResult = result.Result as NoContentResult;
            Assert.Equal(204, noContentResult.StatusCode);
        }
    }
}
