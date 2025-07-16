using Microsoft.Extensions.Logging;
using TaskManagementApp.Application.Mappings;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Models.Projects;

namespace TaskManagementApp.Application.Projects
{
    public class GetAllProjectsService : IGetAllProjectsService
    {
        private readonly ILogger<GetAllProjectsService> _logger;
        private readonly IProjectService _projectService;

        public GetAllProjectsService(ILogger<GetAllProjectsService> logger, IProjectService projectService)
        {
            _logger = logger;
            _projectService = projectService;
        }

        public async Task<IEnumerable<ProjectResponse>> ExecuteAsync()
        {
            _logger.LogInformation("Iniciando a execução de GetAllProjectsService.");

            var projects = await _projectService.GetAllProjectsAsync();
            var response = projects.Select(p => p.ToDto()).ToList();
            
            _logger.LogInformation("Finalizando a execução de GetAllProjectsService.");

            return response;
        }
    }
}
