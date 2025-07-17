using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Enums;

namespace TaskManagementApp.Domain.Interfaces
{
    public interface IProjectTaskService
    {
        /// <summary>
        /// Serviço para criar uma nova tarefa para um projeto específico.
        /// </summary>
        /// <param name="projectExternalId">Id externo do projeto</param>
        /// <param name="title">Título da tarefa</param>
        /// <param name="description">Descrição da tarefa</param>
        /// <param name="deadline">Data limite para a conclusão da tarefa</param>
        /// <param name="priority">Prioridade da tarefa (1-Baixa, 2-Média, 3-Alta)</param>
        /// <param name="userId">Id do usuário responsável pela tarefa</param>
        /// <returns></returns>
        Task<ProjectTask> CreateProjectTaskAsync(Guid projectExternalId, string title, string description, DateTime deadline, ProjectTaskPriority priority, Guid userId);

        /// <summary>
        /// Serviço para obter uma tarefa pelo seu ExternalId.
        /// </summary>
        /// <param name="taskExternalId">ExtenalId da tarefa a ser buscada</param>
        /// <returns>Tarefa encontrada ou null quando não localizar o registro</returns>
        Task<ProjectTask?> GetProjectTaskByExternalIdAsync(Guid taskExternalId);

        /// <summary>
        /// Serviço para obter todas as tarefas de um projeto específico.
        /// </summary>
        /// <param name="projectExternalId">ExternalId do projeto</param>
        /// <returns>Lista de tarefas</returns>
        Task<IEnumerable<ProjectTask>> GetAllProjectTasksByProjectIdAsync(Guid projectExternalId);

        /// <summary>
        /// Serviço para atualizar detalhes de uma tarefa.
        /// </summary>
        /// <param name="taskExternalId">Identificação da tarefa a ser atualizada</param>
        /// <param name="newTitle">Novo título da tarefa</param>
        /// <param name="newDescription">Nova descrição da tarefa</param>
        /// <param name="newDeadline">Nova data limite para a conclusão da tarefa</param>
        /// <param name="modifiedByUserId">Id do usuário que está modificando a tarefa</param>
        /// <returns>Booleano para confirmar se a tarefa foi atualizada</returns>
        Task<bool> UpdateProjectTaskDetailsAsync(Guid taskExternalId, string newTitle, string newDescription, DateTime newDeadline, Guid modifiedByUserId);

        /// <summary>
        /// Serviço para atualizar o status de uma tarefa.
        /// </summary>
        /// <param name="taskExternalId">Identidicação da tarefa a ser atualizada</param>
        /// <param name="newStatus">Novo status da tarefa</param>
        /// <param name="modifiedByUserId">Id do usuário que está modificando a tarefa</param>
        /// <returns>Booleano para confirmar se a tarefa foi atualizada</returns>
        Task<bool> UpdateProjectTaskStatusAsync(Guid taskExternalId, ProjectTaskStatus newStatus, Guid modifiedByUserId);

        /// <summary>
        /// Serviço para remover uma tarefa.
        /// </summary>
        /// <param name="taskExternalId">Identificação da tarefa a ser removida</param>
        /// <returns>Booleano para confirmar se a tarefa foi removida</returns>
        Task<bool> DeleteProjectTaskAsync(Guid taskExternalId);

        /// <summary>
        /// Serviço para obter todas as tarefas.
        /// </summary>
        /// <returns>Retorna lista de tarefas</returns>
        Task<IEnumerable<ProjectTask>> GetAllAsync();
    }
}
