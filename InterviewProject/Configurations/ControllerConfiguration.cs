using FluentValidation.AspNetCore;
using InterviewProject.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewProject.Configurations
{
    public static class ControllerConfiguration
    {
        public static IServiceCollection ControllerSettings(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.EnableEndpointRouting = true;
                options.Filters.Add<ValidationFilter>();
            }).AddNewtonsoftJson(options =>
            {
                options.UseMemberCasing();
            }).AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblyContaining<Startup>();
            });

            return services;
        }
    }
}
