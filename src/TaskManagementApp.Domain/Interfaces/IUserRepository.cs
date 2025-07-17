using TaskManagementApp.Domain.Entities;

namespace TaskManagementApp.Domain.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Adiciona um novo usuário ao repositório.
        /// </summary>
        /// <param name="id">Id do usuário</param>
        /// <returns>Usuário quando localizar na base.</returns>
        Task<User?> GetByExternalIdAsync(Guid id);

        /// <summary>
        /// Consulta usuários do repositório.
        /// </summary>
        /// <param name="id">Id interno do banco de dados.</param>
        /// <returns>Usuário quando localizar na base.</returns>
        Task<User?> GetByIdAsync(int id);

        /// <summary>
        /// Consulta usuários do repositório.
        /// </summary>
        /// <returns>Lista de usuários.</returns>
        Task<IEnumerable<User>> GetAllAsync();
    }
}
