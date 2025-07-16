using Microsoft.Extensions.Logging;
using TaskManagementApp.Application.Mappings;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Application.ProjectTasks
{
    public class UpdateProjectTaskService : IUpdateProjectTaskService
    {
        private readonly IProjectTaskService _projectTaskDomainService;
        private readonly ILogger<UpdateProjectTaskService> _logger;

        public UpdateProjectTaskService(
            IProjectTaskService projectTaskDomainService,
            ILogger<UpdateProjectTaskService> logger)
        {
            _projectTaskDomainService = projectTaskDomainService;
            _logger = logger;
        }
        
        public async Task<ProjectTaskResponse?> ExecuteAsync(Guid id, UpdateProjectTaskRequest request)
        {
            _logger.LogInformation("Iniciando atualização para a tarefa {TaskExternalId}.", id);

            var success = await _projectTaskDomainService.UpdateProjectTaskDetailsAsync(
                id,
                request.Title,
                request.Description,
                request.Deadline
            );

            if (!success)
            {
                _logger.LogWarning("Tarefa {TaskExternalId} não encontrada para atualização de detalhes.", id);
                return null;
            }

            var task = await _projectTaskDomainService.GetProjectTaskByExternalIdAsync(id);

            if (task == null)
            {
                _logger.LogError("Tarefa {TaskExternalId} foi atualizada, mas não pôde ser recuperada após a atualização.", id);
                throw new InvalidOperationException("Tarefa atualizada não pôde ser recuperada.");
            }

            var response = task.ToDto();

            _logger.LogInformation("Detalhes da tarefa '{TaskTitle}' (ID: {TaskId}) atualizados.", task.Title, task.ExternalId);

            return response;
        }
    }
}