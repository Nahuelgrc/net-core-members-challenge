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
    public class ContractorServiceTests
    {
        public async Task<IMemberService<BusinessContractor>> SetupService(MainDbContext mainDbContext = null)
        {
            if (mainDbContext == null)
            {
                mainDbContext = Utils.CreateDbContext();
            }

            await mainDbContext.SaveChangesAsync();

            return new ContractorService(
                mainDbContext,
                Utils.GetMapper(),
                new Mock<ILogger<ContractorService>>().Object);
        }

        [Fact]
        public async Task When_GetAll_IsCalled_Then_ContractorsAreReturned()
        {
            var mainDbContext = Utils.CreateDbContext(true);

            var member1 = FakeData.GetMember(1, JobType.Contractor, "Test1");
            var member2 = FakeData.GetMember(2, JobType.Contractor, "Test2");

            await mainDbContext.AddAsync(member1);
            await mainDbContext.AddAsync(member2);
            await mainDbContext.SaveChangesAsync();

            var membersTags1 = FakeData.GetMembersTags(1, member1.Id, 1);
            var membersTags2 = FakeData.GetMembersTags(2, member2.Id, 2);

            await mainDbContext.AddAsync(membersTags1);
            await mainDbContext.AddAsync(membersTags2);
            await mainDbContext.SaveChangesAsync();

            var newContractor1 = FakeData.GetContractor(1, 3, member1);
            var newContractor2 = FakeData.GetContractor(2, 6, member2);

            await mainDbContext.AddAsync(newContractor1);
            await mainDbContext.AddAsync(newContractor2);
            await mainDbContext.SaveChangesAsync();

            var service = await this.SetupService(mainDbContext);

            var businessContractorResponse = await service.GetAll();

            var firstContractor = businessContractorResponse.Where(x => x.Id == member1.Id)
                .SingleOrDefault();
            var secondContractor = businessContractorResponse.Where(x => x.Id == member2.Id)
                .SingleOrDefault();

            Assert.Equal(2, businessContractorResponse.Count);
            Assert.Equal(newContractor1.ContractDuration, firstContractor.ContractDuration);
            Assert.Equal(member1.Name, firstContractor.Name);
            Assert.Equal(member1.JobType, firstContractor.JobType);
            Assert.Equal(1, firstContractor.Tags.Count);
            Assert.Equal(newContractor2.ContractDuration, secondContractor.ContractDuration);
            Assert.Equal(member2.Name, secondContractor.Name);
            Assert.Equal(member2.JobType, secondContractor.JobType);
            Assert.Equal(1, secondContractor.Tags.Count);
        }

        [Fact]
        public async Task When_GetById_IsCalled_Then_ExceptionIsThrown()
        {
            var service = await this.SetupService();

            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await service.GetById(1));
        }

        [Fact]
        public async Task When_GetById_IsCalled_Then_ContractorIsReturned()
        {
            var mainDbContext = Utils.CreateDbContext(true);

            var member1 = FakeData.GetMember(1, JobType.Contractor, "Test1");

            await mainDbContext.AddAsync(member1);
            await mainDbContext.SaveChangesAsync();

            var membersTags1 = FakeData.GetMembersTags(1, member1.Id, 1);

            await mainDbContext.AddAsync(membersTags1);
            await mainDbContext.SaveChangesAsync();

            var newContractor1 = FakeData.GetContractor(1, 3, member1);

            await mainDbContext.AddAsync(newContractor1);
            await mainDbContext.SaveChangesAsync();

            var service = await this.SetupService(mainDbContext);

            var businessContractorResponse = await service.GetById(member1.Id);

            Assert.Equal(newContractor1.ContractDuration, businessContractorResponse.ContractDuration);
            Assert.Equal(member1.Name, businessContractorResponse.Name);
            Assert.Equal(member1.JobType, businessContractorResponse.JobType);
            Assert.Equal(1, businessContractorResponse.Tags.Count);
        }

        [Fact]
        public async Task When_Create_IsCalled_Then_ContractorIsCreatedInDatabase()
        {
            var newContractor = Builder<BusinessContractor>
                .CreateNew()
                .With(x => x.ContractDuration = 3)
                .With(x => x.Name = "Test")
                .With(x => x.Tags = FakeData.GetBusinessTagList())
                .Build();

            var mainDbContext = Utils.CreateDbContext(true);

            var service = await this.SetupService(mainDbContext);

            var result = await service.Create(newContractor);

            // Check response contractor
            Assert.Equal(1, result.Id);
            Assert.Equal(newContractor.ContractDuration, result.ContractDuration);
            Assert.Equal(newContractor.Name, result.Name);
            Assert.Equal(newContractor.Tags[0].Id, result.Tags[0].Id);
            Assert.Equal(newContractor.Tags[1].Id, result.Tags[1].Id);

            // Check database contractor
            var addedContractor = mainDbContext.Contractors
                .SingleOrDefault();

            Assert.Equal(newContractor.ContractDuration, addedContractor.ContractDuration);
            Assert.Equal(newContractor.Name, addedContractor.Member.Name);
            Assert.Equal(JobType.Contractor, addedContractor.Member.JobType);
            Assert.Equal(newContractor.Tags.Count, addedContractor.Member.MembersAndTags.Count);
        }

        [Fact]
        public async Task When_Update_IsCalled_Then_ExceptionIsThrown()
        {
            var service = await this.SetupService();

            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await service.Update(1, new BusinessContractor()));
        }

        [Fact]
        public async Task When_Update_IsCalled_Then_ContractorIsUpdated()
        {
            var mainDbContext = Utils.CreateDbContext(true);

            var member1 = FakeData.GetMember(1, JobType.Contractor, "Test1");

            await mainDbContext.AddAsync(member1);
            await mainDbContext.SaveChangesAsync();

            var membersTags1 = FakeData.GetMembersTags(1, member1.Id, 1);

            await mainDbContext.AddAsync(membersTags1);
            await mainDbContext.SaveChangesAsync();

            var newContractor1 = FakeData.GetContractor(1, 3, member1);

            await mainDbContext.AddAsync(newContractor1);
            await mainDbContext.AddAsync(FakeData.GetTag(4));
            await mainDbContext.AddAsync(FakeData.GetTag(5));
            await mainDbContext.SaveChangesAsync();

            var service = await this.SetupService(mainDbContext);

            var newBusinessContractor = new BusinessContractor
            {
                ContractDuration = 6,
                Name = "UpdateTest",
                Tags = FakeData.GetBusinessTagList(new int[] { 4, 5 }),
            };

            var result = await service.Update(member1.Id, newBusinessContractor);

            // Check response contractor
            Assert.Equal(1, result.Id);
            Assert.Equal(newBusinessContractor.ContractDuration, result.ContractDuration);
            Assert.Equal(newBusinessContractor.Name, result.Name);
            Assert.Equal(newBusinessContractor.Tags[0].Id, result.Tags[0].Id);
            Assert.Equal(newBusinessContractor.Tags[1].Id, result.Tags[1].Id);

            var contractorInDatabase = mainDbContext.Contractors
                .Where(x => x.Id == member1.Id).SingleOrDefault();

            // Check database contractor
            Assert.Equal(newContractor1.ContractDuration, contractorInDatabase.ContractDuration);
            Assert.Equal(newContractor1.Member.Name, contractorInDatabase.Member.Name);
            Assert.Equal(newContractor1.Member.JobType, contractorInDatabase.Member.JobType);
            Assert.Equal(newContractor1.Member.MembersAndTags.Count, contractorInDatabase.Member.MembersAndTags.Count);
        }

        [Fact]
        public async Task When_Delete_IsCalled_Then_ExceptionIsThrown()
        {
            var service = await this.SetupService();

            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await service.Delete(1));
        }

        [Fact]
        public async Task When_Delete_IsCalled_Then_ContractorIsUpdated()
        {
            var mainDbContext = Utils.CreateDbContext(true);

            var member1 = FakeData.GetMember(1, JobType.Contractor, "Test1");

            await mainDbContext.AddAsync(member1);
            await mainDbContext.SaveChangesAsync();

            var membersTags1 = FakeData.GetMembersTags(1, member1.Id, 1);

            await mainDbContext.AddAsync(membersTags1);
            await mainDbContext.SaveChangesAsync();

            var newContractor1 = FakeData.GetContractor(1, 3, member1);
            await mainDbContext.AddAsync(newContractor1);

            var service = await this.SetupService(mainDbContext);

            await service.Delete(member1.Id);

            var contractorInDatabase = mainDbContext.Contractors
                .Where(x => x.Id == member1.Id).SingleOrDefault();

            Assert.Null(contractorInDatabase);
        }

        [Fact]
        public async Task When_Filter_IsCalled_Then_ContractorsAreReturned()
        {
            var mainDbContext = Utils.CreateDbContext(true);

            var member1 = FakeData.GetMember(1, JobType.Contractor, "Test1");
            var member2 = FakeData.GetMember(2, JobType.Contractor, "Test2");

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

            var newContractor1 = FakeData.GetContractor(1, 3, member1);
            var newContractor2 = FakeData.GetContractor(2, 6, member2);

            await mainDbContext.AddAsync(newContractor1);
            await mainDbContext.AddAsync(newContractor2);
            await mainDbContext.SaveChangesAsync();

            var service = await this.SetupService(mainDbContext);

            var businessContractorResponse = await service.Filter(new int[] { 2, 4 });

            var firstBusinessContractor = businessContractorResponse
                .Where(x => x.Id == member1.Id)
                .SingleOrDefault();

            Assert.Equal(newContractor1.Member.Id, firstBusinessContractor.Id);
            Assert.Equal(newContractor1.Member.Name, firstBusinessContractor.Name);
            Assert.Equal(newContractor1.Member.MembersAndTags.Count, firstBusinessContractor.Tags.Count);
            Assert.Equal(newContractor1.ContractDuration, firstBusinessContractor.ContractDuration);
            Assert.Equal(newContractor1.Member.JobType, firstBusinessContractor.JobType);

            var secondBusinessContractor = businessContractorResponse
                .Where(x => x.Id == member2.Id)
                .SingleOrDefault();

            Assert.Equal(newContractor2.Member.Id, secondBusinessContractor.Id);
            Assert.Equal(newContractor2.Member.Name, secondBusinessContractor.Name);
            Assert.Equal(newContractor2.Member.MembersAndTags.Count, secondBusinessContractor.Tags.Count);
            Assert.Equal(newContractor2.ContractDuration, secondBusinessContractor.ContractDuration);
            Assert.Equal(newContractor2.Member.JobType, secondBusinessContractor.JobType);

            var firstContractor = businessContractorResponse
                .Where(x => x.Id == member1.Id)
                .SingleOrDefault();

            Assert.Equal(newContractor1.Member.Id, firstContractor.Id);
            Assert.Equal(newContractor1.ContractDuration, firstContractor.ContractDuration);
            Assert.Equal(member1.Name, firstContractor.Name);
            Assert.Equal(member1.JobType, firstContractor.JobType);
            Assert.Equal(membersTags1.TagId, firstContractor.Tags[0].Id);
            Assert.Equal(membersTags2.TagId, firstContractor.Tags[1].Id);

            var secondContractor = businessContractorResponse
                .Where(x => x.Id == member2.Id)
                .SingleOrDefault();

            Assert.Equal(newContractor2.Member.Id, secondContractor.Id);
            Assert.Equal(newContractor2.ContractDuration, secondContractor.ContractDuration);
            Assert.Equal(member2.Name, secondContractor.Name);
            Assert.Equal(member2.JobType, secondContractor.JobType);
            Assert.Equal(membersTags3.TagId, secondContractor.Tags[0].Id);
            Assert.Equal(membersTags4.TagId, secondContractor.Tags[1].Id);
        }
    }
}
