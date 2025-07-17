using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Application.ProjectTasks
{
    public interface IUpdateProjectTaskStatusService
    {
        /// <summary>
        /// Atualiza o status de uma tarefa de projeto.
        /// </summary>
        /// <param name="id">Id da tarefa a ser atualizada.</param>
        /// <param name="request">Dados para a atualização do status da tarefa.</param>
        /// <param name="modifiedByUserId">Usuário que realizou a atualização.</param>
        /// <returns>Tarefa atualizada.</returns>
        Task<ProjectTaskResponse?> ExecuteAsync(Guid id, UpdateProjectTaskStatusRequest request, Guid modifiedByUserId);
    }
}
