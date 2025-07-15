using TaskManagementApp.Domain.Entities;

namespace TaskManagementApp.Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task AddAsync(Project project);

        Task<Project?> GetByIdAsync(Guid id);

        Task<IEnumerable<Project>> GetAllAsync();

        void Update(Project project);

        void Delete(Project project);

        // Salva as mudanças pendentes na unidade de trabalho (DbContext)
        Task<int> SaveChangesAsync();
    }
}
