using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Data.Context;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Interfaces;

namespace TaskManagementApp.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByExternalIdAsync(Guid id) => await _context.Users.FirstOrDefaultAsync(u => u.ExternalId.Equals(id));

        public async Task<User?> GetByIdAsync(int id) => await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));

        public async Task<IEnumerable<User>> GetAllAsync() => await _context.Users.ToListAsync();
    }
}
