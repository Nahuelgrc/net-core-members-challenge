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
using InterviewProject.Shared.Exceptions;
using InterviewProject.Tests.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InterviewProject.Tests.Controllers
{
    public class ContractorControllerTests
    {
        private readonly Mock<IMemberService<BusinessContractor>> contractorServiceMock;

        public ContractorControllerTests()
        {
            this.contractorServiceMock = new Mock<IMemberService<BusinessContractor>>();
        }

        public ContractorController SetupController()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<InterviewMappingProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            return new ContractorController(this.contractorServiceMock.Object, mapper);
        }

        [Fact]
        public async Task When_GetAll_Then_ContractorsAreReturned()
        {
            var contractor1 = FakeData.GetBusinessContractor(FakeData.GetBusinessTagList());
            var contractor2 = FakeData.GetBusinessContractor(FakeData.GetBusinessTagList(), 2, 6, "Test2");
            var businessContractors = new List<BusinessContractor>()
            {
                contractor1,
                contractor2,
            };

            this.contractorServiceMock.Setup(x => x.GetAll()).ReturnsAsync(businessContractors);

            var controller = this.SetupController();

            var result = await controller.GetAll();
            var actual = (result.Result as OkObjectResult).Value as ApiGetAllContractorsResponse;

            Assert.Equal(businessContractors.Count, actual.Count);
        }

        [Fact]
        public async Task When_GetById_Then_NotFoundIsReturned()
        {
            var contractor = FakeData.GetBusinessContractor(FakeData.GetBusinessTagList());

            this.contractorServiceMock
                .Setup(x => x.GetById(It.IsAny<int>()))
                .ThrowsAsync(new NotFoundException());

            var controller = this.SetupController();
            var result = await controller.GetById(contractor.Id);
            var notFoundResult = result.Result as NotFoundResult;
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task When_GetById_Then_ContractorIsReturned()
        {
            var contractor = FakeData.GetBusinessContractor(FakeData.GetBusinessTagList());

            this.contractorServiceMock
                .Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(contractor);

            var controller = this.SetupController();
            var result = await controller.GetById(contractor.Id);
            var actual = (result.Result as OkObjectResult).Value as ApiContractorResponse;

            Assert.Equal(contractor.Id, actual.Id);
            Assert.Equal(contractor.Name, actual.Name);
            Assert.Equal(contractor.ContractDuration, actual.ContractDuration);
            Assert.Equal(contractor.Tags.Count, actual.Tags.Count);
        }

        [Fact]
        public async Task When_Create_Then_ContractorIsReturned()
        {
            var request = Builder<ApiContractorRequest>
                .CreateNew()
                .Build();

            var contractor = FakeData.GetBusinessContractor(FakeData.GetBusinessTagList());

            this.contractorServiceMock
                .Setup(x => x.Create(It.IsAny<BusinessContractor>()))
                .ReturnsAsync(contractor);

            var controller = this.SetupController();
            var result = await controller.Create(request);
            var actual = (result.Result as OkObjectResult).Value as ApiContractorResponse;

            Assert.Equal(contractor.Id, actual.Id);
            Assert.Equal(contractor.Name, actual.Name);
            Assert.Equal(contractor.ContractDuration, actual.ContractDuration);
            Assert.Equal(contractor.Tags.Count, actual.Tags.Count);
        }

        [Fact]
        public async Task When_Update_Then_NotFoundIsReturned()
        {
            var contractor = FakeData.GetBusinessContractor(FakeData.GetBusinessTagList());

            var request = Builder<ApiContractorUpdateRequest>
                .CreateNew()
                .Build();

            this.contractorServiceMock
                .Setup(x => x.Update(It.IsAny<int>(), It.IsAny<BusinessContractor>()))
                .ThrowsAsync(new NotFoundException());

            var controller = this.SetupController();
            var result = await controller.Update(contractor.Id, request);
            var notFoundResult = result.Result as NotFoundResult;
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task When_Update_Then_ContractorIsUpdated()
        {
            var contractor = FakeData.GetBusinessContractor(FakeData.GetBusinessTagList());

            var request = Builder<ApiContractorUpdateRequest>
                .CreateNew()
                .Build();

            this.contractorServiceMock
                .Setup(x => x.Update(It.IsAny<int>(), It.IsAny<BusinessContractor>()))
                .ReturnsAsync(contractor);

            var controller = this.SetupController();
            var result = await controller.Update(contractor.Id, request);
            var actual = (result.Result as OkObjectResult).Value as ApiContractorResponse;

            Assert.Equal(contractor.Id, actual.Id);
            Assert.Equal(contractor.Name, actual.Name);
            Assert.Equal(contractor.ContractDuration, actual.ContractDuration);
            Assert.Equal(contractor.Tags.Count, actual.Tags.Count);
        }

        [Fact]
        public async Task When_Delete_Then_NotFoundIsReturned()
        {
            this.contractorServiceMock
                .Setup(x => x.Delete(It.IsAny<int>()))
                .ThrowsAsync(new NotFoundException());

            var controller = this.SetupController();
            var result = await controller.Delete(1);
            var notFoundResult = result.Result as NotFoundResult;
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task When_Delete_Then_ContractorIsUpdated()
        {
            this.contractorServiceMock
                .Setup(x => x.Delete(It.IsAny<int>()));

            var controller = this.SetupController();
            var result = await controller.Delete(1);
            var noContentResult = result.Result as NoContentResult;
            Assert.Equal(204, noContentResult.StatusCode);
        }
    }
}
