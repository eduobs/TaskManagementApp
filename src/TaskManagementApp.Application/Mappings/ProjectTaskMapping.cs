using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Application.Mappings
{
    public static class ProjectTaskMapping
    {
        public static ProjectTaskResponse ToDto(this ProjectTask projectTask)
        {
            ArgumentNullException.ThrowIfNull(projectTask);

            return new ProjectTaskResponse
            {
                Id = projectTask.ExternalId,
                Title = projectTask.Title,
                Description = projectTask.Description,
                Deadline = projectTask.Deadline,
                Status = (Models.Enums.ProjectTaskStatus)projectTask.Status,
                Priority = (Models.Enums.ProjectTaskPriority)projectTask.Priority,
                ProjectId = projectTask.Project.ExternalId
            };
        }
    }
}
