using ChallengerTest.Data;

namespace ChallengerTest.Repositories
{
    public interface IUnitOfWork
    {
        IPermissionRepository Permissions { get; }
        Task SaveAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IPermissionRepository _permissionRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IPermissionRepository Permissions => _permissionRepository ??= new PermissionRepository(_context);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}