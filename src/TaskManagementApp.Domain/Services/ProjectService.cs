using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Interfaces;

namespace TaskManagementApp.Domain.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
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
            var project = await _projectRepository.GetByIdAsync(externalId);
            if (project == null)
                return false;

            _projectRepository.Delete(project);
            await _projectRepository.SaveChangesAsync();

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
