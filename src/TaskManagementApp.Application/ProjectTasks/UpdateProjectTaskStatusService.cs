using Microsoft.Extensions.Logging;
using TaskManagementApp.Application.Mappings;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Application.ProjectTasks
{
    public class UpdateProjectTaskStatusService : IUpdateProjectTaskStatusService
    {
        private readonly IProjectTaskService _projectTaskDomainService;
        private readonly ILogger<UpdateProjectTaskStatusService> _logger;

        public UpdateProjectTaskStatusService(
            IProjectTaskService projectTaskDomainService,
            ILogger<UpdateProjectTaskStatusService> logger)
        {
            _projectTaskDomainService = projectTaskDomainService;
            _logger = logger;
        }

        public async Task<ProjectTaskResponse?> ExecuteAsync(Guid id, UpdateProjectTaskStatusRequest request)
        {
            _logger.LogInformation(
                "Iniciando atualização de status para a tarefa {TaskExternalId} com status {NewStatus}.",
                id,
                request.Status
            );

            var success = await _projectTaskDomainService.UpdateProjectTaskStatusAsync(
                id,
                (Domain.Enums.ProjectTaskStatus)request.Status
            );

            if (!success)
            {
                _logger.LogWarning("Tarefa com ExternalId {TaskExternalId} não encontrada para atualização de status.", id);
                return null;
            }

            var updatedTask = await _projectTaskDomainService.GetProjectTaskByExternalIdAsync(id);

            if (updatedTask == null)
            {
                _logger.LogError("Tarefa {TaskExternalId} teve o status atualizado, mas não pôde ser recuperada após a atualização.", id);
                throw new InvalidOperationException("Tarefa atualizada não pôde ser recuperada.");
            }

            var response = updatedTask.ToDto();

            _logger.LogInformation(
                "Status da tarefa '{TaskTitle}' (ID: {TaskId}) atualizado para {Status} e mapeado para resposta.",
                updatedTask.Title,
                updatedTask.ExternalId,
                response.Status
            );

            return response;
        }
    }
}
