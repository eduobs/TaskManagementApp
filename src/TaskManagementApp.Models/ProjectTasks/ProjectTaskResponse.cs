using TaskManagementApp.Models.Enums;

namespace TaskManagementApp.Models.ProjectTasks
{
    public class ProjectTaskResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public ProjectTaskStatus Status { get; set; }
        public ProjectTaskPriority Priority { get; set; }
        public Guid ProjectId { get; set; }
    }
}
