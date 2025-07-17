using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Data.Context;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Interfaces;

namespace TaskManagementApp.Data.Repositories
{
    public class ProjectTaskHistoryRepository : IProjectTaskHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectTaskHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProjectTaskHistory historyEntry) => await _context.ProjectTaskHistories.AddAsync(historyEntry);

        public async Task<IEnumerable<ProjectTaskHistory>> GetHistoryForTaskAsync(Guid taskExternalId)
        {
            return await _context.ProjectTaskHistories
                                .Include(pth => pth.ProjectTask)
                                .Where(pth => pth.ProjectTask.ExternalId == taskExternalId)
                                .OrderByDescending(pth => pth.ModificationDate)
                                .ToListAsync();
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
