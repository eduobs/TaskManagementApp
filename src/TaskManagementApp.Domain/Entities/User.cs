using TaskManagementApp.Domain.Enums;

namespace TaskManagementApp.Domain.Entities
{
    public class User
    {
        private User() { }

        public User(string name, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome do usuário não pode ser nulo ou vazio.", nameof(name));

            ExternalId = Guid.NewGuid();
            Name = name;
            Role = role;
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; private set; }

        public Guid ExternalId { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public UserRole Role { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public void Update(string newName, UserRole newRole)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("O nome do usuário não pode ser nulo ou vazio.", nameof(newName));

            Name = newName;
            Role = newRole;
        }
    }
}
