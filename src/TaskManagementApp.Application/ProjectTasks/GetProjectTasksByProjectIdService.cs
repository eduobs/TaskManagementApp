using Microsoft.Extensions.Logging;
using TaskManagementApp.Application.Mappings;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Application.ProjectTasks
{
    public class GetProjectTasksByProjectIdService : IGetProjectTasksByProjectIdService
    {
        private readonly IProjectTaskService _projectTaskDomainService;
        private readonly ILogger<GetProjectTasksByProjectIdService> _logger;

        public GetProjectTasksByProjectIdService(IProjectTaskService projectTaskDomainService, ILogger<GetProjectTasksByProjectIdService> logger)
        {
            _projectTaskDomainService = projectTaskDomainService;
            _logger = logger;
        }

        public async Task<IEnumerable<ProjectTaskResponse>> ExecuteAsync(Guid projectExternalId)
        {
            _logger.LogInformation("Iniciando de tarefas do projeto {ProjectExternalId}.", projectExternalId);

            var projectTasks = await _projectTaskDomainService.GetAllProjectTasksByProjectIdAsync(projectExternalId);

            if (projectTasks == null || !projectTasks.Any())
            {
                _logger.LogInformation("Nenhuma tarefa encontrada para o projeto {ProjectExternalId}.", projectExternalId);
                return [];
            }

            var response = projectTasks.Select(pt => pt.ToDto()).ToList();

            _logger.LogInformation("Obtidas {TaskCount} tarefas para o projeto {ProjectExternalId}.", response.Count, projectExternalId);

            return response;
        }
    }
}
