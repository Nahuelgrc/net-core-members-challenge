namespace InterviewProject.Configurations
{
    using InterviewProject.Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class DatabaseConfiguration
    {
        private const string InMemoryDatabaseProvider = "Microsoft.EntityFrameworkCore.InMemory";

        public static IServiceCollection DatabaseSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("ConnectionString");
            services.AddDbContext<MainDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            return services;
        }

        public static void RunDatabaseMigrations(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                return;
            }

            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
            if (serviceScope == null)
            {
                return;
            }

            var candidateDbContextContext = serviceScope.ServiceProvider.GetRequiredService<MainDbContext>();
            if (candidateDbContextContext.Database.ProviderName != InMemoryDatabaseProvider)
            {
                candidateDbContextContext.Database.Migrate();
            }
        }
    }
}