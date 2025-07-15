using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementApp.Domain.Entities;

namespace TaskManagementApp.Data.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects");

            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.Id)
                    .ValueGeneratedOnAdd();

            builder.Property(p => p.ExternalId)
                    .IsRequired();
                    
            builder.HasIndex(p => p.ExternalId)
                    .IsUnique();

            builder.Property(p => p.Name)
                    .HasMaxLength(255)
                    .IsRequired();

            builder.Property(p => p.Description)
                    .HasMaxLength(1000)
                       .IsRequired();
        }
    }
}