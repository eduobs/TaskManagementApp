using Microsoft.Extensions.DependencyInjection;
using TaskManagementApp.Application.Projects;
using TaskManagementApp.Application.ProjectTasks;
using TaskManagementApp.Application.Reports;

namespace TaskManagementApp.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICreateProjectService, CreateProjectService>();
            services.AddScoped<IGetProjectService, GetProjectService>();
            services.AddScoped<IGetAllProjectsService, GetAllProjectsService>();
            services.AddScoped<ICreateProjectTaskService, CreateProjectTaskService>();
            services.AddScoped<IGetProjectTasksByProjectIdService, GetProjectTasksByProjectIdService>();
            services.AddScoped<IUpdateProjectTaskService, UpdateProjectTaskService>();
            services.AddScoped<IUpdateProjectTaskStatusService, UpdateProjectTaskStatusService>();
            services.AddScoped<IDeleteProjectTaskService, DeleteProjectTaskService>();
            services.AddScoped<IDeleteProjectService, DeleteProjectService>();
            services.AddScoped<IGetPerformanceReportService, GetPerformanceReportService>();
            services.AddScoped<IAddCommentToTaskService, AddCommentToTaskService>();
            
            return services;
        }
    }
}
