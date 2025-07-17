using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Application.ProjectTasks
{
    public interface IUpdateProjectTaskService
    {
        /// <summary>
        /// Atualiza uma tarefa de projeto.
        /// </summary>
        /// <param name="id">Id da tarefa a ser atualizada.</param>
        /// <param name="request">Dados para a atualização da tarefa.</param>
        /// <param name="modifiedByUserId">Usuário que realizou a atualização.</param>
        /// <returns>Tarefa atualizada.</returns>
        Task<ProjectTaskResponse?> ExecuteAsync(Guid id, UpdateProjectTaskRequest request, Guid modifiedByUserId);
    }
}
