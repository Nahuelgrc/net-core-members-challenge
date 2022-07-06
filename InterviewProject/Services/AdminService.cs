using InterviewProject.Data;
using InterviewProject.Data.Models;
using InterviewProject.Services.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace InterviewProject.Services
{
    public class AdminService : IAdminService
    {
        private readonly MainDbContext mainDbContext;
        private readonly ILogger<AdminService> logger;

        public AdminService(MainDbContext mainDbContext, ILogger<AdminService> logger)
        {
            this.mainDbContext = mainDbContext;
            this.logger = logger;
        }

        public async Task AddTag(string name)
        {
            try
            {
                await this.mainDbContext.Tags.AddAsync(new Tag
                {
                    Name = name
                });

                await this.mainDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError($"AdminService.GetAll - {ex.Message}");
                throw ex;
            }
        }
    }
}
