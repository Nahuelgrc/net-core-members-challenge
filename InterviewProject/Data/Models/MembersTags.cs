using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace InterviewProject.Data.Models
{
    public class MembersTags
    {
        [Key]
        public int Id { get; set; }

        public int MemberId { get; set; }

        public Member Member { get; set; }

        public int TagId { get; set; }

        public Tag Tag { get; set; }
    }

    public class MembersTagsConfiguration : IEntityTypeConfiguration<MembersTags>
    {
        public void Configure(EntityTypeBuilder<MembersTags> builder)
        {
            builder.HasIndex(t => t.Id);
        }
    }
}
