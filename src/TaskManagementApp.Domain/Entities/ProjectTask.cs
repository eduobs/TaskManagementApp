using TaskManagementApp.Domain.Enums;

namespace TaskManagementApp.Domain.Entities
{
    public class ProjectTask
    {
        private ProjectTask() { }

        public ProjectTask(string title, string description, DateTime deadline, ProjectTaskPriority priority, int projectId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("O título da tarefa não pode ser nulo ou vazio.", nameof(title));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("A descrição da tarefa não pode ser nula ou vazia.", nameof(description));

            if (deadline < DateTime.Today)
                throw new ArgumentException("A data de vencimento não pode ser no passado.", nameof(deadline));

            if (projectId <= 0)
                throw new ArgumentException("O ID do projeto deve ser válido.", nameof(projectId));


            ExternalId = Guid.NewGuid();
            Title = title;
            Description = description;
            Deadline = deadline;
            Priority = priority;
            Status = ProjectTaskStatus.Pending;
            ProjectId = projectId;
        }

        public int Id { get; private set; }

        public Guid ExternalId { get; private set; }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public DateTime Deadline { get; private set; }

        public ProjectTaskStatus Status { get; private set; }

        public ProjectTaskPriority Priority { get; private set; }

        public int ProjectId { get; private set; }

        public Project Project { get; private set; } = null!;

        public void UpdateDetails(string title, string description, DateTime deadline)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("O título da tarefa não pode ser nulo ou vazio.", nameof(title));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("A descrição da tarefa não pode ser nula ou vazia.", nameof(description));

            if (deadline < DateTime.Today && Status != ProjectTaskStatus.Completed)
                throw new ArgumentException("A data de vencimento não pode ser no passado.", nameof(deadline));

            Title = title;
            Description = description;
            Deadline = deadline;
        }

        public void UpdateStatus(ProjectTaskStatus status) => Status = status;
    }
}
