using TaskManagementApp.Models.Projects;

namespace TaskManagementApp.Application.Projects
{
    public interface IGetAllProjectsService
    {
        Task<IEnumerable<ProjectResponse>> ExecuteAsync();
    }
}
