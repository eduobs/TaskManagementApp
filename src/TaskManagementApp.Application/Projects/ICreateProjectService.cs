using TaskManagementApp.Models.Projects;

namespace TaskManagementApp.Application.Projects
{
    public interface ICreateProjectService
    {
        /// <summary>
        /// Cria um novo projeto.
        /// </summary>
        /// <param name="request">Dados para criar um novo projeto</param>
        /// <param name="userId">Id do usuário que está criando o projeto</param>
        /// <returns>Projeto que foi criado</returns>
        Task<ProjectResponse> ExecuteAsync(CreateProjectRequest request, Guid userId);
    }
}
