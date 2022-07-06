using AutoMapper;
using InterviewProject.Helpers;
using InterviewProject.MappingConfiguration;
using InterviewProject.Services;
using InterviewProject.Services.Abstractions;
using InterviewProject.Services.Models;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewProject.Configurations
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection ServiceSettings(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new InterviewMappingProfile());
            });

            var mapper = mapperConfig.CreateMapper();

            services.AddSingleton(mapper);
            services.AddSingleton<JwtService>(new JwtService("SecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecretSecret"));
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMemberService<BusinessContractor>, ContractorService>();
            services.AddScoped<IMemberService<BusinessEmployee>, EmployeeService>();
            services.AddScoped<IAdminService, AdminService>();

            return services;
        }
    }
}
