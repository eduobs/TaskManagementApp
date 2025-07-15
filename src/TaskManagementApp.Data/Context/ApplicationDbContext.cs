using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Data.Configurations;
using TaskManagementApp.Domain.Entities;

namespace TaskManagementApp.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet para a entidade Project
        public DbSet<Project> Projects { get; set; }

         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        }
    }
}