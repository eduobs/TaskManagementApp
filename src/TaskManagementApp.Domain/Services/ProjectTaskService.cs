using Microsoft.Extensions.Logging;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Enums;
using TaskManagementApp.Domain.Interfaces;

namespace TaskManagementApp.Domain.Services
{
    public class ProjectTaskService : IProjectTaskService
    {
        private readonly ILogger<ProjectTaskService> _logger;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly IProjectTaskHistoryRepository _projectTaskHistoryRepository;

        public ProjectTaskService(
            ILogger<ProjectTaskService> logger,
            IProjectRepository projectRepository,
            IProjectTaskRepository projectTaskRepository,
            IProjectTaskHistoryRepository projectTaskHistoryRepository)
        {
            _logger = logger;
            _projectRepository = projectRepository;
            _projectTaskRepository = projectTaskRepository;
            _projectTaskHistoryRepository = projectTaskHistoryRepository;
        }

        public async Task<ProjectTask> CreateProjectTaskAsync(Guid projectExternalId, string title, string description, DateTime deadline, ProjectTaskPriority priority)
        {
            _logger.LogInformation("Iniciando criação da tarefa para o projeto {ProjectExternalId}.", projectExternalId);

            var project = await _projectRepository.GetByIdAsync(projectExternalId);
            if (project == null)
            {
                _logger.LogWarning("Projeto com ExternalId {ProjectExternalId} não encontrado.", projectExternalId);
                throw new ArgumentException($"Projeto com ID {projectExternalId} não encontrado.");
            }

            var currentTaskCount = await _projectTaskRepository.CountTasksByProjectIdAsync(project.Id);
            if (currentTaskCount >= 20)
            {
                _logger.LogWarning("Tentativa de adicionar tarefa ao projeto {ProjectName} (ID: {ProjectId}) falhou: limite de 20 tarefas atingido.", project.Name, project.ExternalId);
                throw new InvalidOperationException($"O projeto '{project.Name}' atingiu o limite máximo de 20 tarefas.");
            }

            var projectTask = new ProjectTask(title, description, deadline, priority, project.Id);

            await _projectTaskRepository.AddAsync(projectTask);
            await _projectTaskRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Tarefa '{TaskTitle}' (ID: {TaskId}) criada com sucesso para o projeto {ProjectName} (ID: {ProjectId}).",
                projectTask.Title,
                projectTask.ExternalId,
                project.Name,
                project.ExternalId
            );

            return projectTask;
        }

        public async Task<ProjectTask?> GetProjectTaskByExternalIdAsync(Guid taskExternalId)
        {
            _logger.LogInformation("Buscando tarefa com ExternalId: {TaskExternalId}.", taskExternalId);
            return await _projectTaskRepository.GetByIdAsync(taskExternalId);
        }

        public async Task<IEnumerable<ProjectTask>> GetAllProjectTasksByProjectIdAsync(Guid projectExternalId)
        {
            _logger.LogInformation("Buscando todas as tarefas para o projeto com ExternalId: {ProjectExternalId}.", projectExternalId);
            return await _projectTaskRepository.GetAllByProjectIdAsync(projectExternalId);
        }

        public async Task<bool> UpdateProjectTaskDetailsAsync(Guid taskExternalId, string newTitle, string newDescription, DateTime newDeadline, Guid modifiedByUserId)
        {
            _logger.LogInformation("Iniciando atualização de detalhes para a tarefa com ExternalId: {TaskExternalId}.", taskExternalId);
            var projectTask = await _projectTaskRepository.GetByIdAsync(taskExternalId);
            if (projectTask == null)
            {
                _logger.LogWarning("Tarefa com ExternalId {TaskExternalId} não encontrada para atualização.", taskExternalId);
                return false;
            }

            // Capturar valores antigos antes da atualização
            var oldTitle = projectTask.Title;
            var oldDescription = projectTask.Description;
            var oldDeadline = projectTask.Deadline;

            try
            {
                projectTask.UpdateDetails(newTitle, newDescription, newDeadline);
                _projectTaskRepository.Update(projectTask);
                await _projectTaskRepository.SaveChangesAsync();

                await AddHistoryEntryForDetailsUpdate(projectTask.Id, oldTitle, newTitle, oldDescription, newDescription, oldDeadline, newDeadline, modifiedByUserId);

                _logger.LogInformation("Detalhes da tarefa {TaskExternalId} atualizados com sucesso.", taskExternalId);

                return true;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao atualizar detalhes da tarefa {TaskExternalId}: {Message}", taskExternalId, ex.Message);
                throw;
            }
        }

        public async Task<bool> UpdateProjectTaskStatusAsync(Guid taskExternalId, ProjectTaskStatus newStatus, Guid modifiedByUserId)
        {
            _logger.LogInformation("Iniciando atualização de status para a tarefa com ExternalId: {TaskExternalId} para status {NewStatus}.", taskExternalId, newStatus);

            var projectTask = await _projectTaskRepository.GetByIdAsync(taskExternalId);
            if (projectTask == null)
            {
                _logger.LogWarning("Tarefa com ExternalId {TaskExternalId} não encontrada para atualização de status.", taskExternalId);
                return false;
            }

            var oldStatus = projectTask.Status;

            projectTask.UpdateStatus(newStatus);
            _projectTaskRepository.Update(projectTask);
            await _projectTaskRepository.SaveChangesAsync();

            await AddHistoryEntryForStatusUpdate(projectTask.Id, oldStatus, newStatus, modifiedByUserId);

            _logger.LogInformation("Status da tarefa {TaskExternalId} atualizado com sucesso para {NewStatus}.", taskExternalId, newStatus);

            return true;
        }

        public async Task<bool> DeleteProjectTaskAsync(Guid taskExternalId)
        {
            _logger.LogInformation("Iniciando exclusão da tarefa com ExternalId: {TaskExternalId}.", taskExternalId);

            var projectTask = await _projectTaskRepository.GetByIdAsync(taskExternalId);
            if (projectTask == null)
            {
                _logger.LogWarning("Tarefa com ExternalId {TaskExternalId} não encontrada para exclusão.", taskExternalId);
                return false;
            }

            _projectTaskRepository.Delete(projectTask);
            await _projectTaskRepository.SaveChangesAsync();

            _logger.LogInformation("Tarefa {TaskExternalId} excluída com sucesso.", taskExternalId);

            return true;
        }

        private async Task AddHistoryEntryForDetailsUpdate(int projectTaskId, string oldTitle, string newTitle, string oldDescription, string newDescription, DateTime oldDeadline, DateTime newDeadline, Guid modifiedByUserId)
        {
            var historyEntriesToAdd = new List<ProjectTaskHistory>();

            if (oldTitle != newTitle)
                historyEntriesToAdd.Add(ProjectTaskHistory.Create(projectTaskId, "Title", oldTitle, newTitle, modifiedByUserId));

            if (oldDescription != newDescription)
                historyEntriesToAdd.Add(ProjectTaskHistory.Create(projectTaskId, "Description", oldDescription, newDescription, modifiedByUserId));

            if (oldDeadline != newDeadline)
                historyEntriesToAdd.Add(ProjectTaskHistory.Create(projectTaskId, "Deadline", oldDeadline.ToString("yyyy-MM-dd"), newDeadline.ToString("yyyy-MM-dd"), modifiedByUserId));

            if (historyEntriesToAdd.Count != 0)
            {
                foreach (var entry in historyEntriesToAdd)
                    await _projectTaskHistoryRepository.AddAsync(entry);

                await _projectTaskHistoryRepository.SaveChangesAsync();
            }
        }

        private async Task AddHistoryEntryForStatusUpdate(int projectTaskId, ProjectTaskStatus oldStatus, ProjectTaskStatus newStatus, Guid modifiedByUserId)
        {
            if (oldStatus != newStatus)
            {
                await _projectTaskHistoryRepository.AddAsync(ProjectTaskHistory.Create(projectTaskId, "Status", oldStatus.ToString(), newStatus.ToString(), modifiedByUserId));
                await _projectTaskHistoryRepository.SaveChangesAsync();
            }
        }
    }
}
