using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Models.Projects;
using Microsoft.Extensions.Logging;
using TaskManagementApp.Application.Mappings;

namespace TaskManagementApp.Application.Projects
{
    public class CreateProjectService : ICreateProjectService
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<CreateProjectService> _logger;


        public CreateProjectService(ILogger<CreateProjectService> logger, IProjectService projectService)
        {
            _logger = logger;
            _projectService = projectService;
        }

        public async Task<ProjectResponse> ExecuteAsync(CreateProjectRequest request, Guid userId)
        {
            _logger.LogInformation(
                "Recebida requisição para criar projeto: {ProjectName}",
                request.Name
            );

            var project = await _projectService.CreateProjectAsync(
                request.Name,
                request.Description,
                userId
            );

            var response = project.ToDto();

            _logger.LogInformation(
                "Projeto '{ProjectName}' (ID: {ProjectId}) criado com sucesso.",
                project.Name, project.ExternalId
            );

            return response;
        }
    }
}
