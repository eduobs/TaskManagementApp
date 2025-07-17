namespace TaskManagementApp.Application.Projects
{
    public interface IDeleteProjectService
    {
        /// <summary>
        /// Deleta um projeto pelo seu id.
        /// </summary>
        /// <param name="projectExternalId">Id do projeto a ser deletado</param>
        /// <returns>Booleano indicando se o projeto foi deletado com sucesso</returns>
        Task<bool> ExecuteAsync(Guid projectExternalId);
    }
}
