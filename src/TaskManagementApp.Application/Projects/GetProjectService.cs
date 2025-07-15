using Microsoft.Extensions.Logging;
using TaskManagementApp.Application.Mappings;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Models.Projects;

namespace TaskManagementApp.Application.Projects
{
    public class GetProjectService : IGetProjectService
    {
        private readonly ILogger<GetProjectService> _logger;
        private readonly IProjectService _projectService;

        public GetProjectService(ILogger<GetProjectService> logger, IProjectService projectService)
        {
            _logger = logger;
            _projectService = projectService;
        }

        public async Task<ProjectResponse> GetProjectByExternalIdAsync(Guid id)
        {
            var project = await _projectService.GetProjectByExternalIdAsync(id);

            if (project == null)
            {
                _logger.LogInformation("Projeto com ID: {ProjectId} não encontrado.", id);
                return null;
            }

            return project.ToDto();
        }
    }
}
