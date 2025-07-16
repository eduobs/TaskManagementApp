using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Data.Context;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Interfaces;

namespace TaskManagementApp.Data.Repositories
{
    public class ProjectTaskRepository : IProjectTaskRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectTaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProjectTask projectTask) => await _context.ProjectTasks.AddAsync(projectTask);

        public async Task<ProjectTask?> GetByIdAsync(Guid id)
        {
            return await _context.ProjectTasks
                                 .Include(pt => pt.Project)
                                 .FirstOrDefaultAsync(pt => pt.ExternalId.Equals(id));
        }

        public async Task<IEnumerable<ProjectTask>> GetAllByProjectIdAsync(Guid projectId)
        {
            return await _context.ProjectTasks
                                .Where(pt => pt.Project.ExternalId.Equals(projectId))
                                .Include(pt => pt.Project)
                                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectTask>> GetAllAsync() => await _context.ProjectTasks.ToListAsync();

        public void Update(ProjectTask projectTask) => _context.ProjectTasks.Update(projectTask);

        public void Delete(ProjectTask projectTask) => _context.ProjectTasks.Remove(projectTask);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<int> CountTasksByProjectIdAsync(int id) => await _context.ProjectTasks.CountAsync(pt => pt.ProjectId.Equals(id));
    }
}
