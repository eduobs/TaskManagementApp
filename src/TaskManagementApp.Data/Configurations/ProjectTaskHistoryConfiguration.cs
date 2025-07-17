using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementApp.Domain.Entities;

namespace TaskManagementApp.Data.Configurations
{
    public class ProjectTaskHistoryConfiguration : IEntityTypeConfiguration<ProjectTaskHistory>
    {
        public void Configure(EntityTypeBuilder<ProjectTaskHistory> builder)
        {
            builder.ToTable("ProjectTaskHistories");

            builder.HasKey(pth => pth.Id);

            builder.Property(pth => pth.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(pth => pth.ProjectTaskId)
                   .IsRequired();

            builder.Property(pth => pth.PropertyName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(pth => pth.OldValue)
                   .HasMaxLength(1000);

            builder.Property(pth => pth.NewValue)
                   .HasMaxLength(1000);

            builder.Property(pth => pth.ModificationDate)
                   .IsRequired();

            builder.Property(pth => pth.ModifiedByUserId)
                   .IsRequired();

            builder.Property(pth => pth.ChangeType)
                   .HasMaxLength(50);

            builder.HasOne(pth => pth.ProjectTask)
                   .WithMany()
                   .HasForeignKey(pth => pth.ProjectTaskId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
