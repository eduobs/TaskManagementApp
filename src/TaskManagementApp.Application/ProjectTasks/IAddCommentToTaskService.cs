using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Application.ProjectTasks
{
    public interface IAddCommentToTaskService
    {
        Task<bool> ExecuteAsync(Guid taskExternalId, AddCommentToTaskRequest request, Guid commentedByUserId);
    }
}
