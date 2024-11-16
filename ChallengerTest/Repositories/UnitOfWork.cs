using ChallengerTest.Data;
using ChallengerTest.Repositories.Command;
using ChallengerTest.Repositories.Query;
using Nest;
using System.Threading.Tasks;

namespace ChallengerTest.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IElasticClient _elasticClient;
        private IPermissionQueryRepository _permissionQueryRepository;
        private IPermissionCommandRepository _permissionCommandRepository;

        public UnitOfWork(ApplicationDbContext context, IElasticClient elasticClient)
        {
            _context = context;
            _elasticClient = elasticClient;
        }

        public IPermissionQueryRepository PermissionQueries =>
            _permissionQueryRepository ??= new PermissionQueryRepository(_elasticClient);

        public IPermissionCommandRepository PermissionCommands =>
            _permissionCommandRepository ??= new PermissionCommandRepository(_context, _elasticClient);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();  // Guarda los cambios en la base de datos
        }
    }
}