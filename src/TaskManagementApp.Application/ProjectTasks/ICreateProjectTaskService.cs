using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Application.ProjectTasks
{
    public interface ICreateProjectTaskService
    {
        /// <summary>
        /// Serviço cria uma nova tarefa de projeto.
        /// </summary>
        /// <param name="projectExternalId">Id do projeto ao qual a tarefa pertence.</param>
        /// <param name="request">Dados para a criação da tarefa.</param>
        /// <returns>Tarefa que foi criada.</returns>
        Task<ProjectTaskResponse> ExecuteAsync(Guid projectExternalId, CreateProjectTaskRequest request);
    }
}
