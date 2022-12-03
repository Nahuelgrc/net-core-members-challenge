using InterviewProject.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace InterviewProject.Data.Models
{
    public class Auth : IdentityUser
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public AuthRoleType AuthRoleType { get; set; }
    }

    public class AuthConfiguration : IEntityTypeConfiguration<Auth>
    {
        public void Configure(EntityTypeBuilder<Auth> builder)
        {
            builder.HasIndex(t => t.Id);
            builder.HasIndex(t => t.Username).IsUnique();
            builder.Property(t => t.Password).IsRequired();
            builder.Property(t => t.AuthRoleType).IsRequired();

            builder.HasData(new Auth
            {
                Id = 1,
                Username = "admin",
                Password = "$2a$11$Qp1aeTjKvn4kCaydAe4C1uPVaL4fi.5Dxjy/OKb6kPWHSfQwWs4N6",
                AuthRoleType = AuthRoleType.Admin
            });
        }
    }
}
