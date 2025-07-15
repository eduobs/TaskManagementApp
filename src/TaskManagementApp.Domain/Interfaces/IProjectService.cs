using TaskManagementApp.Domain.Entities;

namespace TaskManagementApp.Domain.Interfaces
{
    public interface IProjectService
    {
        /// <summary>
        /// Serviço para criar um novo projeto.
        /// </summary>
        /// <param name="name">Nome do projeto</param>
        /// <param name="description">Descrição do projeto</param>
        /// <returns>Retorna o projeto criado</returns>
        Task<Project> CreateProjectAsync(string name, string description);

        /// <summary>
        /// Serviço para obter um projeto pelo seu ExternalId (Guid).
        /// </summary>
        /// <param name="externalId">Id externo do projeto desejado</param>
        /// <returns>Quando conseguir obter um projeto pelo IdExterno retorna o projeto</returns>
        Task<Project?> GetProjectByExternalIdAsync(Guid externalId);

        /// <summary>
        /// Serviço para listar todos os projetos.
        /// </summary>
        /// <returns>Lista de projetos</returns>
        Task<IEnumerable<Project>> GetAllProjectsAsync();

        /// <summary>
        /// Serviço para atualizar um projeto (nome, descrição)
        /// </summary>
        /// <param name="externalId">Id externo do projeto a ser atualizado</param>
        /// <param name="newName">Novo nome do projeto</param>
        /// <param name="newDescription">Nova descrição do projeto</param>
        /// <returns></returns>
        Task<bool> UpdateProjectAsync(Guid externalId, string newName, string newDescription);

        /// <summary>
        /// Serviço para remover um projeto.
        /// </summary>
        /// <param name="externalId">Identificação do projeto a ser removido</param>
        /// <returns>Booleano para confirmar se o projeto foi removido</returns>
        Task<bool> DeleteProjectAsync(Guid externalId);
    }
}
