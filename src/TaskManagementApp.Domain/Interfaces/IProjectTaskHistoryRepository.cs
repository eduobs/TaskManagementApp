using TaskManagementApp.Domain.Entities;

namespace TaskManagementApp.Domain.Interfaces
{
    public interface IProjectTaskHistoryRepository
    {
        /// <summary>
        /// Adiciona uma nova entrada de histórico.
        /// </summary>
        /// <param name="historyEntry">Dados do histórico a ser adicionado</param>
        Task AddAsync(ProjectTaskHistory historyEntry);

        /// <summary>
        /// Consulta histórico de tarefas.
        /// </summary>
        /// <param name="taskExternalId">Id da tarefa</param>
        /// <returns>Lista de históricos</returns>
        Task<IEnumerable<ProjectTaskHistory>> GetHistoryForTaskAsync(Guid taskExternalId);

        /// <summary>
        /// Salva as mudanças pendentes no repositório.
        /// </summary>
        /// <returns>Identificador do registro salvo.</returns>
        Task<int> SaveChangesAsync();
    }
}
