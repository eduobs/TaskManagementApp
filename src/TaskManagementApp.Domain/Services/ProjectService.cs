using Microsoft.Extensions.Logging;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Enums;
using TaskManagementApp.Domain.Interfaces;

namespace TaskManagementApp.Domain.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ILogger<ProjectService> _logger;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectTaskRepository _projectTaskRepository;


        public ProjectService(
            ILogger<ProjectService> logger,
            IProjectRepository projectRepository,
            IProjectTaskRepository projectTaskRepository)
        {
            _logger = logger;
            _projectRepository = projectRepository;
            _projectTaskRepository = projectTaskRepository;
        }

        public async Task<Project> CreateProjectAsync(string name, string description)
        {
            var project = new Project(name, description);

            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveChangesAsync();

            return project;
        }

        public async Task<bool> DeleteProjectAsync(Guid externalId)
        {
            _logger.LogInformation("Iniciando exclusão do projeto com ExternalId: {ExternalId}.", externalId);

            var project = await _projectRepository.GetByIdAsync(externalId);
            if (project == null)
            {
                _logger.LogWarning("Projeto com ExternalId {ExternalId} não encontrado para exclusão.", externalId);
                return false;
            }

            var pendingTasksCount = await _projectTaskRepository.CountTasksByProjectIdAsync(project.Id);
            var projectTasks = await _projectTaskRepository.GetAllByProjectIdAsync(externalId);

            var hasPendingTasks = projectTasks.Any(t => t.Status == ProjectTaskStatus.Pending);

            if (hasPendingTasks)
            {
                _logger.LogWarning("Tentativa de excluir projeto {ProjectName} (id: {ProjectId}) falhou: Contém tarefas pendentes.", project.Name, project.ExternalId);
                throw new InvalidOperationException($"Não é possível remover o projeto '{project.Name}'. Ainda há tarefas pendentes associadas a ele. Conclua ou remova as tarefas primeiro.");
            }

            _projectRepository.Delete(project);
            await _projectRepository.SaveChangesAsync();

            _logger.LogInformation("Projeto com id: {ExternalId} removido com sucesso.", externalId);

            return true;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync() => await _projectRepository.GetAllAsync();

        public async Task<Project?> GetProjectByExternalIdAsync(Guid externalId) => await _projectRepository.GetByIdAsync(externalId);

        public async Task<bool> UpdateProjectAsync(Guid externalId, string newName, string newDescription)
        {
            var project = await _projectRepository.GetByIdAsync(externalId);
            if (project == null)
                return false;

            project.UpdateName(newName);
            project.UpdateDescription(newDescription);

            _projectRepository.Update(project);
            await _projectRepository.SaveChangesAsync();

            return true;
        }
    }
}
