namespace TaskManagementApp.Application.ProjectTasks
{
    public interface IDeleteProjectTaskService
    {
        /// <summary>
        /// Serviço que deleta uma tarefa.
        /// </summary>
        /// <param name="id">Id da tarefa a ser deletada.</param>
        /// <returns>Booleano indicando se a tarefa foi deletada com sucesso.</returns>
        Task<bool> ExecuteAsync(Guid id);
    }
}
