using AutoMapper;
using InterviewProject.Data;
using InterviewProject.Data.Models;
using InterviewProject.Helpers;
using InterviewProject.Services.Abstractions;
using InterviewProject.Services.Models;
using InterviewProject.Shared;
using InterviewProject.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace InterviewProject.Services
{
    public class AuthService : IAuthService
    {
        private readonly MainDbContext mainDbContext;
        private readonly IMapper mapper;
        private readonly ILogger<AuthService> logger;
        private readonly JwtService jwtService;

        public AuthService(MainDbContext mainDbContext, IMapper mapper, ILogger<AuthService> logger, JwtService jwtService)
        {
            this.mainDbContext = mainDbContext;
            this.mapper = mapper;
            this.logger = logger;
            this.jwtService = jwtService;
        }

        public async Task<BusinessLogin> Login(string username, string password)
        {
            var user = this.mainDbContext.Auths
                .Where(x => x.Username.Equals(username))
                .SingleOrDefault();

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new NotFoundException();
            }

            return new BusinessLogin
            {
                Id = user.Id,
                RoleType = user.AuthRoleType.ToString(),
                Token = this.jwtService.Generate(user.Id, user.AuthRoleType)
            };
        }

        public async Task<BusinessLogin> RegisterAndLogin(string username, string password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            await this.mainDbContext.AddAsync(new Auth
            {
                Username = username,
                AuthRoleType = AuthRoleType.Worker,
                Password = hashedPassword
            });

            var id = await this.mainDbContext.SaveChangesAsync();

            return new BusinessLogin
            {
                Id = id,
                RoleType = AuthRoleType.Worker.ToString(),
                Token = this.jwtService.Generate(id, AuthRoleType.Worker)
            };
        }
    }
}
