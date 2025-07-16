using Microsoft.Extensions.Logging;
using TaskManagementApp.Domain.Interfaces;

namespace TaskManagementApp.Application.ProjectTasks
{
    public class DeleteProjectTaskService : IDeleteProjectTaskService
    {
        private readonly ILogger<DeleteProjectTaskService> _logger;
        private readonly IProjectTaskService _projectTaskDomainService;

        public DeleteProjectTaskService(
            ILogger<DeleteProjectTaskService> logger,
            IProjectTaskService projectTaskDomainService)
        {
            _logger = logger;
            _projectTaskDomainService = projectTaskDomainService;
        }

        public async Task<bool> ExecuteAsync(Guid taskExternalId)
        {
            _logger.LogInformation("Iniciando a exclusão da tarefa {TaskExternalId}.", taskExternalId);

            var success = await _projectTaskDomainService.DeleteProjectTaskAsync(taskExternalId);

            if (!success)
            {
                _logger.LogWarning("Tarefa com ExternalId {TaskExternalId} não encontrada para exclusão.", taskExternalId);
                return false;
            }

            _logger.LogInformation("Tarefa com id: {TaskId} excluída com sucesso.", taskExternalId);

            return true;
        }
    }
}
