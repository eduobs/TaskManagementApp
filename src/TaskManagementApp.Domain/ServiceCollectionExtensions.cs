using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManagementApp.Domain
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            // Register domain services
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IProjectTaskService, ProjectTaskService>();
            
            return services;
        }
    }
}
