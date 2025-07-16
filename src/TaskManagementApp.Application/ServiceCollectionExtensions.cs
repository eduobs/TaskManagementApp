using Microsoft.Extensions.DependencyInjection;
using TaskManagementApp.Application.Projects;

namespace TaskManagementApp.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICreateProjectService, CreateProjectService>();
            services.AddScoped<IGetProjectService, GetProjectService>();
            services.AddScoped<IGetAllProjectsService, GetAllProjectsService>();

            return services;
        }
    }
}
