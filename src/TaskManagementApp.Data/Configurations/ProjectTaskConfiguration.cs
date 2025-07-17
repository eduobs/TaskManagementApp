using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementApp.Domain.Entities;

namespace TaskManagementApp.Data.Configurations
{
    public class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
    {
        public void Configure(EntityTypeBuilder<ProjectTask> builder)
        {
            builder.ToTable("ProjectTasks");

            builder.HasKey(pt => pt.Id);

            builder.Property(pt => pt.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(pt => pt.ExternalId)
                   .IsRequired();

            builder.HasIndex(pt => pt.ExternalId)
                   .IsUnique();

            builder.Property(pt => pt.Title)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(pt => pt.Description)
                   .HasMaxLength(1000)
                   .IsRequired();

            builder.Property(pt => pt.Deadline)
                   .IsRequired();

            builder.Property(pt => pt.Status)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(pt => pt.Priority)
                   .HasConversion<int>()
                   .IsRequired();

            // Configura o relacionamento um-para-muitos com Project
            builder.HasOne(pt => pt.Project)
                   .WithMany(p => p.Tasks)
                   .HasForeignKey(pt => pt.ProjectId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(pt => pt.CreatedAt)
                    .IsRequired();

            builder.Property(pt => pt.UpdatedAt)
                   .IsRequired();

            builder.Property(pt => pt.AssignedToUserId)
                   .IsRequired();

            builder.HasOne(pt => pt.AssignedToUser)
                   .WithMany()
                   .HasForeignKey(pt => pt.AssignedToUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
