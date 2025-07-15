using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Models.Projects;

namespace TaskManagementApp.Application.Mappings
{
    public static class ProjectMapping
    {
        public static ProjectResponse ToDto(this Project project)
        {
            ArgumentNullException.ThrowIfNull(project);

            return new ProjectResponse
            {
                Id = project.ExternalId,
                Name = project.Name,
                Description = project.Description
            };
        }
    }
}
