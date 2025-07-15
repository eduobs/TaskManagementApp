using TaskManagementApp.Models.Projects;

namespace TaskManagementApp.Application.Projects
{
    public interface IGetProjectService
    {
        /// <summary>
        /// Obtém um projeto pelo seu ID.
        /// </summary>
        /// <param name="id">Id do projeto a ser recuperado</param>
        /// <returns>Projeto</returns>
        Task<ProjectResponse> GetProjectByExternalIdAsync(Guid id);
    }
}
