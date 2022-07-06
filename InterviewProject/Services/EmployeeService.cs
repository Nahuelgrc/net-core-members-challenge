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
    public class EmployeeService : IMemberService<BusinessEmployee>
    {
        private readonly MainDbContext mainDbContext;
        private readonly IMapper mapper;
        private readonly ILogger<EmployeeService> logger;

        public EmployeeService(MainDbContext mainDbContext, IMapper mapper, ILogger<EmployeeService> logger)
        {
            this.mainDbContext = mainDbContext;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<IList<BusinessEmployee>> GetAll()
        {
            try
            {
                var employees = this.mainDbContext
                    .GetEmployeesListWithMemberDataQueryable();

                return this.mapper.Map<IList<BusinessEmployee>>(employees);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"EmployeeService.GetAll - {ex.Message}");
                throw ex;
            }
        }

        public async Task<BusinessEmployee> GetById(int id)
        {
            try
            {
                var employee = this.mainDbContext
                    .GetEmployeeWithMemberDataQueryable(x => x.Member.Id == id);

                if (employee == null)
                {
                    throw new NotFoundException();
                }

                return this.mapper.Map<BusinessEmployee>(employee);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"EmployeeService.GetById - {ex.Message}");
                throw ex;
            }
        }

        public async Task<BusinessEmployee> Create(BusinessEmployee businessEmployee)
        {
            using var transaction = await this.mainDbContext.Database.BeginTransactionAsync();
            try
            {
                // Add Member
                var member = this.mapper.Map<Member>(businessEmployee);
                member.JobType = JobType.Employee;
                await this.mainDbContext.AddAsync(member);

                // Add Tags
                var tagIds = businessEmployee.Tags.Select(x => x.Id).ToList();
                await this.mainDbContext.AddTags(member, tagIds);

                // Add Employee
                var employee = this.mapper.Map<Employee>(businessEmployee);
                employee.Member = member;
                await this.mainDbContext.AddAsync(employee);

                await this.mainDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return this.mapper.Map<BusinessEmployee>(employee);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"EmployeeService.Create - {ex.Message}");
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task<BusinessEmployee> Update(int id, BusinessEmployee businessEmployee)
        {
            using var transaction = await this.mainDbContext.Database.BeginTransactionAsync();
            try
            {
                var employeeToUpdate = this.mainDbContext
                    .GetEmployeeWithMemberDataQueryable(x => x.Member.Id == id);

                if (employeeToUpdate == null)
                {
                    throw new NotFoundException();
                }

                if (businessEmployee.Tags != null)
                {
                    this.mainDbContext.RemoveRange(employeeToUpdate.Member.MembersAndTags);

                    if (businessEmployee.Tags.Count > 0)
                    {
                        var tagIds = businessEmployee.Tags.Select(x => x.Id).ToList();
                        await this.mainDbContext.AddTags(employeeToUpdate.Member, tagIds);
                    }
                }

                // Update Data
                employeeToUpdate.RoleType = businessEmployee.RoleType;
                employeeToUpdate.Member.Name = businessEmployee.Name;

                this.mainDbContext.Update(employeeToUpdate);
                this.mainDbContext.Update(employeeToUpdate.Member);

                await this.mainDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return this.mapper.Map<BusinessEmployee>(employeeToUpdate);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                this.logger.LogError($"EmployeeService.Update - {ex.Message}");
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task Delete(int id)
        {
            using var transaction = await this.mainDbContext.Database.BeginTransactionAsync();
            try
            {
                var employee = this.mainDbContext
                    .GetEmployeeWithMemberDataQueryable(x => x.Member.Id == id);

                if (employee == null)
                {
                    throw new NotFoundException();
                }

                this.mainDbContext.MembersAndTags.RemoveRange(employee.Member.MembersAndTags);
                this.mainDbContext.Employees.Remove(employee);
                this.mainDbContext.Members.Remove(employee.Member);

                await this.mainDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                this.logger.LogError($"EmployeeService.Delete - {ex.Message}");
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task<IList<BusinessEmployee>> Filter(IList<int> flagIds)
        {
            try
            {
                var membersIds = this.mainDbContext.MembersAndTags
                    .Include(x => x.Member)
                    .Where(x => flagIds.Contains(x.TagId))
                    .Select(x => x.MemberId)
                    .ToList();

                var employees = this.mainDbContext.GetEmployeesListWithMemberDataQueryable(x => membersIds.Contains(x.Member.Id));

                return this.mapper.Map<IList<BusinessEmployee>>(employees);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"EmployeeService.Filter - {ex.Message}");
                throw ex;
            }
        }
    }
}
