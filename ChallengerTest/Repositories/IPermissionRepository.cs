using ChallengerTest.Data;
using ChallengerTest.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengerTest.Repositories
{
    public interface IPermissionRepository
    {
        Task<Permission> GetPermissionByIdAsync(int id);
        Task AddPermissionAsync(Permission permission);
        Task SaveAsync();
    }

    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDbContext _context;

        public PermissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Permission> GetPermissionByIdAsync(int id)
        {
            return await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddPermissionAsync(Permission permission)
        {
            await _context.Permissions.AddAsync(permission);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}