using InterviewProject.Data.Models;
using InterviewProject.Shared;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterviewProject.Data
{
    public class MainDbContext : IdentityDbContext<Auth>
    {
        public MainDbContext(DbContextOptions<MainDbContext> options)
            : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }

        public DbSet<Contractor> Contractors { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<MembersTags> MembersAndTags { get; set; }

        public DbSet<Auth> Auths { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MemberConfiguration());
            modelBuilder.ApplyConfiguration(new ContractorConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new MembersTagsConfiguration());
            modelBuilder.ApplyConfiguration(new AuthConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public async Task AddTags(Member member, List<int> tagIds)
        {
            member.MembersAndTags = new List<MembersTags>();

            // Get tag ids from the database to validate that they exist
            var tagDict = await this.Tags
                .Where(x => tagIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, y => y);

            foreach (var item in tagDict.Values)
            {
                member.MembersAndTags.Add(new MembersTags
                {
                    MemberId = member.Id,
                    TagId = item.Id,
                });
            }
        }

        public IList<Contractor> GetContractorsListWithMemberDataQueryable(Func<Contractor, bool> predicate = null)
        {
            var query = this.GetContractorsQueryable(predicate);
            return query.ToList();
        }

        public Contractor GetContractorWithMemberDataQueryable(Func<Contractor, bool> predicate = null)
        {
            var query = this.GetContractorsQueryable(predicate);
            return query.SingleOrDefault();
        }

        public IList<Employee> GetEmployeesListWithMemberDataQueryable(Func<Employee, bool> predicate = null)
        {
            var query = this.GetEmployeesQueryable(predicate);
            return query.ToList();
        }

        public Employee GetEmployeeWithMemberDataQueryable(Func<Employee, bool> predicate = null)
        {
            var query = this.GetEmployeesQueryable(predicate);
            return query.SingleOrDefault();
        }

        private IQueryable<Contractor> GetContractorsQueryable(Func<Contractor, bool> predicate = null)
        {
            var query = this.Contractors
                .Include(x => x.Member)
                .Include(x => x.Member.MembersAndTags)
                .Where(x => x.Member.JobType.Equals(JobType.Contractor))
                .AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate).AsQueryable();
            }

            return query;
        }

        private IQueryable<Employee> GetEmployeesQueryable(Func<Employee, bool> predicate = null)
        {
            var query = this.Employees
                .Include(x => x.Member)
                .Include(x => x.Member.MembersAndTags)
                .Where(x => x.Member.JobType.Equals(JobType.Employee))
                .AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate).AsQueryable();
            }

            return query;
        }
    }
}
