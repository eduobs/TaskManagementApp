using Microsoft.Extensions.DependencyInjection;
using TaskManagementApp.Application.Projects;

namespace TaskManagementApp.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICreateProjectService, CreateProjectService>();
            services.AddScoped<IGetProjectService, GetProjectService>();

            return services;
        }
    }
}
