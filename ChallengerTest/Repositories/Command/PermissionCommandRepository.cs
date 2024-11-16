using ChallengerTest.Data;
using ChallengerTest.Models;
using Nest;
using System.Threading.Tasks;

namespace ChallengerTest.Repositories.Command
{
    public class PermissionCommandRepository : IPermissionCommandRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IElasticClient _elasticClient;

        public PermissionCommandRepository(ApplicationDbContext context, IElasticClient elasticClient)
        {
            _context = context;
            _elasticClient = elasticClient;
        }

        public async Task AddPermissionAsync(Permission permission)
        {
            await _context.Permissions.AddAsync(permission);  // Agregar permiso a la base de datos
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();  // Guardar cambios en la base de datos
        }

        public async Task IndexPermissionAsync(Permission permission)
        {
            var response = await _elasticClient.IndexDocumentAsync(permission);  // Indexar en Elasticsearch

            if (!response.IsValid)
            {
                throw new Exception($"Error indexing permission in Elasticsearch: {response.OriginalException.Message}");
            }
        }
    }
}