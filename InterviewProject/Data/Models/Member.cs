using InterviewProject.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InterviewProject.Data.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public JobType JobType { get; set; }

        public ICollection<MembersTags> MembersAndTags { get; set; }
    }

    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.HasIndex(t => t.Id);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
            builder.Property(t => t.JobType).IsRequired();
        }
    }
}
