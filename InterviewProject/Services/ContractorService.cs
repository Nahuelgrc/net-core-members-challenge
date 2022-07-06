using AutoMapper;
using InterviewProject.Data;
using InterviewProject.Data.Models;
using InterviewProject.Services.Abstractions;
using InterviewProject.Services.Models;
using InterviewProject.Shared;
using InterviewProject.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterviewProject.Services
{
    public class ContractorService : IMemberService<BusinessContractor>
    {
        private readonly MainDbContext mainDbContext;
        private readonly IMapper mapper;
        private readonly ILogger<ContractorService> logger;

        public ContractorService(MainDbContext mainDbContext, IMapper mapper, ILogger<ContractorService> logger)
        {
            this.mainDbContext = mainDbContext;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<IList<BusinessContractor>> GetAll()
        {
            try
            {
                var contractors = this.mainDbContext
                    .GetContractorsListWithMemberDataQueryable();

                return this.mapper.Map<IList<BusinessContractor>>(contractors);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"ContractorService.GetAll - {ex.Message}");
                throw ex;
            }
        }

        public async Task<BusinessContractor> GetById(int id)
        {
            try
            {
                var contractor = this.mainDbContext
                    .GetContractorWithMemberDataQueryable(x => x.Member.Id == id);

                if (contractor == null)
                {
                    throw new NotFoundException();
                }

                return this.mapper.Map<BusinessContractor>(contractor);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                this.logger.LogError($"ContractorService.GetById - {ex.Message}");
                throw ex;
            }
        }

        public async Task<BusinessContractor> Create(BusinessContractor businessContractor)
        {
            using var transaction = await this.mainDbContext.Database.BeginTransactionAsync();
            try
            {
                // Add Member
                var member = this.mapper.Map<Member>(businessContractor);
                member.JobType = JobType.Contractor;
                await this.mainDbContext.AddAsync(member);

                // Add Tags
                var tagIds = businessContractor.Tags.Select(x => x.Id).ToList();
                await this.mainDbContext.AddTags(member, tagIds);

                // Add Contractor
                var contractor = this.mapper.Map<Contractor>(businessContractor);
                contractor.Member = member;
                await this.mainDbContext.AddAsync(contractor);

                await this.mainDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return this.mapper.Map<BusinessContractor>(contractor);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"ContractorService.Create - {ex.Message}");
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task<BusinessContractor> Update(int id, BusinessContractor businessContractor)
        {
            using var transaction = await this.mainDbContext.Database.BeginTransactionAsync();
            try
            {
                var contractorToUpdate = this.mainDbContext
                    .GetContractorWithMemberDataQueryable(x => x.Member.Id == id);

                if (contractorToUpdate == null)
                {
                    throw new NotFoundException();
                }

                if (businessContractor.Tags != null)
                {
                    this.mainDbContext.RemoveRange(contractorToUpdate.Member.MembersAndTags);

                    if (businessContractor.Tags.Count > 0)
                    {
                        var tagIds = businessContractor.Tags.Select(x => x.Id).ToList();
                        await this.mainDbContext.AddTags(contractorToUpdate.Member, tagIds);
                    }
                }

                // Update Data
                contractorToUpdate.ContractDuration = businessContractor.ContractDuration;
                contractorToUpdate.Member.Name = businessContractor.Name;

                this.mainDbContext.Update(contractorToUpdate);
                this.mainDbContext.Update(contractorToUpdate.Member);

                await this.mainDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return this.mapper.Map<BusinessContractor>(contractorToUpdate);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                this.logger.LogError($"ContractorService.Update - {ex.Message}");
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task Delete(int id)
        {
            using var transaction = await this.mainDbContext.Database.BeginTransactionAsync();
            try
            {
                var contractor = this.mainDbContext
                    .GetContractorWithMemberDataQueryable(x => x.Member.Id == id);

                if (contractor == null)
                {
                    throw new NotFoundException();
                }

                this.mainDbContext.MembersAndTags.RemoveRange(contractor.Member.MembersAndTags);
                this.mainDbContext.Contractors.Remove(contractor);
                this.mainDbContext.Members.Remove(contractor.Member);

                await this.mainDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                this.logger.LogError($"ContractorService.Delete - {ex.Message}");
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task<IList<BusinessContractor>> Filter(IList<int> flagIds)
        {
            try
            {
                var membersIds = this.mainDbContext.MembersAndTags
                    .Include(x => x.Member)
                    .Where(x => flagIds.Contains(x.TagId))
                    .Select(x => x.MemberId)
                    .ToList();

                var contractors = this.mainDbContext.GetContractorsListWithMemberDataQueryable(x => membersIds.Contains(x.Member.Id));

                return this.mapper.Map<IList<BusinessContractor>>(contractors);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"ContractorService.Filter - {ex.Message}");
                throw ex;
            }
        }
    }
}
