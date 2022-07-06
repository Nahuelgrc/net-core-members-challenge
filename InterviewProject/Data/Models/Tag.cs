using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InterviewProject.Data.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<MembersTags> MembersAndTags { get; set; }
    }

    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasIndex(t => t.Id);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(100);

            builder.HasData(
                new Tag
                {
                    Id = 1,
                    Name = "C#",
                },
                new Tag
                {
                    Id = 2,
                    Name = "Python",
                },
                new Tag
                {
                    Id = 3,
                    Name = "Ruby",
                },
                new Tag
                {
                    Id = 4,
                    Name = "Java",
                },
                new Tag
                {
                    Id = 5,
                    Name = "Angular",
                },
                new Tag
                {
                    Id = 6,
                    Name = "NodeJS",
                },
                new Tag
                {
                    Id = 7,
                    Name = "NetCore",
                },
                new Tag
                {
                    Id = 8,
                    Name = "Flutter",
                },
                new Tag
                {
                    Id = 9,
                    Name = "React Native",
                },
                new Tag
                {
                    Id = 10,
                    Name = "C++",
                },
                new Tag
                {
                    Id = 11,
                    Name = "JavaScript",
                });
        }
    }
}
