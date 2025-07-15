using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Data.Context;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Interfaces;

namespace TaskManagementApp.Data.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
        }

        public void Delete(Project project)
        {
            _context.Projects.Remove(project);
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _context.Projects.ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(Guid id)
        {
            return await _context.Projects.FirstOrDefaultAsync(p => p.ExternalId == id);
        }

        public void Update(Project project)
        {
            _context.Projects.Update(project);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
