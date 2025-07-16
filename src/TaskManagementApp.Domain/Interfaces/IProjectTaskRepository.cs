using TaskManagementApp.Domain.Entities;

namespace TaskManagementApp.Domain.Interfaces
{
    public interface IProjectTaskRepository
    {
        /// <summary>
        /// Adiciona uma nova tarefa ao repositório.
        /// </summary>
        /// <param name="projectTask">Tarefa a ser adicionada</param>
        Task AddAsync(ProjectTask projectTask);

        /// <summary>
        /// Busca uma tarefas pelo seu ID externo (GUID).
        /// </summary>
        /// <param name="id">Id da tarefa a ser buscada</param>
        /// <returns>Tarefa encontrada ou null quando não localizar o registro</returns>
        Task<ProjectTask?> GetByIdAsync(Guid id);

        // 
        /// <summary>
        /// Busca todas as tarefas de um projeto específico pelo ExternalId do projeto.
        /// </summary>
        /// <param name="projectId">ExternalId do projeto</param>
        /// <returns>Lista de tarefas</returns>
        Task<IEnumerable<ProjectTask>> GetAllByProjectIdAsync(Guid projectId);

        /// <summary>
        /// Busca todas as tarefas do repositório.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ProjectTask>> GetAllAsync();

        /// <summary>
        /// Atualiza uma tarefa existente no repositório.
        /// </summary>
        /// <param name="projectTask">Tarefa a ser atualizada</param>
        void Update(ProjectTask projectTask);

        /// <summary>
        /// Deleta uma tarefa do repositório.
        /// </summary>
        /// <param name="projectTask">Tarefa a ser removida</param>
        void Delete(ProjectTask projectTask);

        /// <summary>
        /// Salva as mudanças pendentes no repositório.
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();

        // Novo: Conta o número de tarefas para um dado projeto.
        /// <summary>
        /// Conta o número de tarefas de um dado projeto.
        /// </summary>
        /// <param name="id">Id do projeto</param>
        /// <returns>Número de tarefas</returns>
        Task<int> CountTasksByProjectIdAsync(int id);
    }
}
