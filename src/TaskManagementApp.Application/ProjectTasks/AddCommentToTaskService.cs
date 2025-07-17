using Microsoft.Extensions.Logging;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Enums;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Application.ProjectTasks
{
    public class AddCommentToTaskService : IAddCommentToTaskService
    {
        private readonly ILogger<AddCommentToTaskService> _logger;
        private readonly IProjectTaskService _projectTaskDomainService;
        private readonly IProjectTaskHistoryRepository _projectTaskHistoryRepository;
        private readonly IUserRepository _userRepository;

        public AddCommentToTaskService(
            ILogger<AddCommentToTaskService> logger,
            IProjectTaskService projectTaskDomainService,
            IProjectTaskHistoryRepository projectTaskHistoryRepository,
            IUserRepository userRepository)
        {
            _logger = logger;
            _projectTaskDomainService = projectTaskDomainService;
            _projectTaskHistoryRepository = projectTaskHistoryRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> ExecuteAsync(Guid taskExternalId, AddCommentToTaskRequest request, Guid commentedByUserId)
        {
            _logger.LogInformation("Iniciando inclusão de comentário para a tarefa {TaskExternalId} pelo usuário {CommentedByUserId}.", taskExternalId, commentedByUserId);

            var projectTask = await _projectTaskDomainService.GetProjectTaskByExternalIdAsync(taskExternalId);
            if (projectTask == null)
            {
                _logger.LogWarning("Tarefa com ExternalId {TaskExternalId} não encontrada para adicionar comentário.", taskExternalId);
                return false;
            }

            if (commentedByUserId.Equals(Guid.Empty))
            {
                _logger.LogWarning("O id do usuário que comentou é inválido (Guid.Empty).");
                throw new ArgumentException("O id do usuário que comentou é obrigatório.", nameof(commentedByUserId));
            }

            var commentingUser = await _userRepository.GetByExternalIdAsync(commentedByUserId);
            if (commentingUser == null)
            {
                 _logger.LogWarning("Usuário com ExternalId {CommentedByUserId} não encontrado. Não será possível registrar o comentário.", commentedByUserId);
                 throw new ArgumentException($"Usuário com id {commentedByUserId} não encontrado para registrar comentário.");
            }

            var historyEntry = ProjectTaskHistory.Create(
                projectTaskId: projectTask.Id,
                propertyName: "Comment",
                oldValue: "",
                newValue: request.CommentContent,
                modifiedByUserId: commentedByUserId,
                changeType: HistoryChangeType.CommentAdded
            );

            await _projectTaskHistoryRepository.AddAsync(historyEntry);
            await _projectTaskHistoryRepository.SaveChangesAsync();

            _logger.LogInformation("Comentário adicionado com sucesso à tarefa {TaskExternalId} pelo usuário {CommentedByUserId}.", taskExternalId, commentedByUserId);

            return true;
        }
    }
}