namespace TaskManagementApp.Domain.Entities
{
    public class Project
    {
        private Project() { }

        public Project(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome do projeto não pode ser nulo ou vazio.", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("A descrição do projeto não pode ser nula ou vazia.", nameof(description));

            ExternalId = Guid.NewGuid();
            Name = name;
            Description = description;
        }

        public int Id { get; private set; }

        public Guid ExternalId { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public ICollection<ProjectTask> Tasks { get; private set; } = [];

        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("O nome do projeto não pode ser nulo ou vazio.", nameof(newName));

            Name = newName;
        }

        public void UpdateDescription(string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
                throw new ArgumentException("A descrição do projeto não pode ser nula ou vazia.", nameof(newDescription));

            Description = newDescription;
        }
        
        public void AddTask(ProjectTask task)
        {
            ArgumentNullException.ThrowIfNull(task);

            if (Tasks.Count >= 20)
                throw new InvalidOperationException("O projeto atingiu o limite máximo de 20 tarefas.");

            if (task.ProjectId != this.Id)
                throw new InvalidOperationException("A tarefa não pertence a este projeto.");

            Tasks.Add(task);
        }
    }
}
