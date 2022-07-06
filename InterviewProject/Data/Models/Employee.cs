using InterviewProject.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace InterviewProject.Data.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        public Member Member { get; set; }

        public EmployeeRoleType RoleType { get; set; }
    }

    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasIndex(t => t.Id);
            builder.Property(t => t.RoleType).IsRequired();
        }
    }
}
