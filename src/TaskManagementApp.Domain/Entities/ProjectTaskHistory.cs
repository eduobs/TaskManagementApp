namespace TaskManagementApp.Domain.Entities
{
    public class ProjectTaskHistory
    {
        private ProjectTaskHistory() { }

        public ProjectTaskHistory(int projectTaskId, string propertyName, string oldValue, string newValue, Guid modifiedByUserId, string? changeType = null)
        {
            if (projectTaskId <= 0)
                throw new ArgumentException("O id interno da tarefa é inválido.", nameof(projectTaskId));

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("O nome da propriedade modificada é obrigatório.", nameof(propertyName));

            if (modifiedByUserId == Guid.Empty)
                throw new ArgumentException("O ID do usuário modificador é obrigatório.", nameof(modifiedByUserId));

            ProjectTaskId = projectTaskId;
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
            ModificationDate = DateTime.UtcNow;
            ModifiedByUserId = modifiedByUserId;

            ChangeType = string.IsNullOrWhiteSpace(changeType) ? "Update" : changeType;
        }
        
        public int Id { get; private set; }

        public int ProjectTaskId { get; private set; }

        public ProjectTask ProjectTask { get; private set; } = null!;

        public string PropertyName { get; private set; } = string.Empty;

        public string OldValue { get; private set; } = string.Empty;

        public string NewValue { get; private set; } = string.Empty;

        public DateTime ModificationDate { get; private set; }

        public Guid ModifiedByUserId { get; private set; }

        public string? ChangeType { get; private set; }

        public static ProjectTaskHistory Create(int projectTaskId, string propertyName, string oldValue, string newValue, Guid modifiedByUserId, string? changeType = null)
        {
            return new ProjectTaskHistory(projectTaskId, propertyName, oldValue, newValue, modifiedByUserId, changeType);
        }
    }
}
