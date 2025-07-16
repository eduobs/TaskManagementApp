using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Application.ProjectTasks
{
    public interface IGetProjectTasksByProjectIdService
    {
        /// <summary>
        /// Consulta tarefas de um projeto.
        /// </summary>
        /// <param name="projectExternalId">Identificador do projeto</param>
        /// <returns>Lista de tarefas do projeto.</returns>
        Task<IEnumerable<ProjectTaskResponse>> ExecuteAsync(Guid projectExternalId);
    }
}
