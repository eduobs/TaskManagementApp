using Microsoft.Extensions.Logging;
using TaskManagementApp.Application.Mappings;
using TaskManagementApp.Domain.Enums;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Application.ProjectTasks
{
    public class CreateProjectTaskService : ICreateProjectTaskService
    {
        private readonly IProjectTaskService _projectTaskDomainService;
        private readonly ILogger<CreateProjectTaskService> _logger;

        public CreateProjectTaskService(
            IProjectTaskService projectTaskDomainService,
            ILogger<CreateProjectTaskService> logger)
        {
            _projectTaskDomainService = projectTaskDomainService;
            _logger = logger;
        }

        public async Task<ProjectTaskResponse> ExecuteAsync(Guid projectExternalId, CreateProjectTaskRequest request)
        {
            _logger.LogInformation("Iniciando a execução criação de uma tarefa para o projeto {ProjectExternalId}.", projectExternalId);

            var projectTask = await _projectTaskDomainService.CreateProjectTaskAsync(
                projectExternalId,
                request.Title,
                request.Description,
                request.Deadline,
                (ProjectTaskPriority)request.Priority
            );

            var response = projectTask.ToDto();

            _logger.LogInformation("Tarefa '{TaskTitle}' (ID: {TaskId}) criada com sucesso.", projectTask.Title, projectTask.ExternalId);

            return response;
        }
    }
}
