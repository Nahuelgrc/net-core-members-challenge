using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace InterviewProject.Data.Models
{
    public class Contractor
    {
        [Key]
        public int Id { get; set; }

        public Member Member { get; set; }

        public int ContractDuration { get; set; }
    }

    public class ContractorConfiguration : IEntityTypeConfiguration<Contractor>
    {
        public void Configure(EntityTypeBuilder<Contractor> builder)
        {
            builder.HasIndex(t => t.Id);
            builder.Property(t => t.ContractDuration).IsRequired();
        }
    }
}
