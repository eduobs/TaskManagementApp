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
        
        // ID interno para o banco de dados (chave primária)
        public int Id { get; private set; }

        // ID externo, exposto via API, para referências públicas
        public Guid ExternalId { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        // public ICollection<Task> Tasks { get; private set; } = [];

        // Métodos para atualizar propriedades (exemplo)
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
    }
}
