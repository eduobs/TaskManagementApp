using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Enums;

namespace TaskManagementApp.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(u => u.ExternalId)
                   .IsRequired();

            builder.HasIndex(u => u.ExternalId)
                   .IsUnique();

            builder.Property(u => u.Name)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(u => u.Role)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(u => u.CreatedAt)
                   .IsRequired();

            // *** Configuração para Seeding de Dados ***
            // Isso irá popular a tabela Users com esses dados quando uma migração for aplicada.
            // Os IDs internos (Id) não precisam ser definidos aqui, mas o ExternalId sim.
            builder.HasData(
                new
                {
                    Id = 1, // IDs internos devem ser únicos e consistentes para seeding
                    ExternalId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Name = "Usuario Padrao",
                    Role = UserRole.Basic,
                    CreatedAt = DateTime.Parse("2025-01-01T10:00:00Z").ToUniversalTime()
                },
                new
                {
                    Id = 2,
                    ExternalId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Name = "Gerente Geral",
                    Role = UserRole.Manager,
                    CreatedAt = DateTime.Parse("2024-01-01T10:00:00Z").ToUniversalTime()
                }
            );
        }
    }
}
