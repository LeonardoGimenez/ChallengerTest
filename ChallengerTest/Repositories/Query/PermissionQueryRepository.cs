using ChallengerTest.Models;
using Elasticsearch.Net;
using Nest;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChallengerTest.Repositories.Query
{
    public class PermissionQueryRepository : IPermissionQueryRepository
    {
        private readonly IElasticClient _elasticClient;

        public PermissionQueryRepository(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<List<Permission>> GetPermissionsAsync()
        {
            var response = await _elasticClient.SearchAsync<Permission>(s => s
                .Index("permissions")
                .Query(q => q
                    .MatchAll()  // Devuelve todos los permisos
                )
            );

            if (!response.IsValid)
            {
                throw new Exception($"Error retrieving permissions from Elasticsearch: {response.OriginalException.Message}");
            }

            return response.Documents.ToList();
        }

        public async Task<Permission> GetPermissionByIdAsync(int id)
        {
            var response = await _elasticClient.GetAsync<Permission>(id, g => g.Index("permissions"));

            if (!response.IsValid)
            {
                throw new Exception($"Error retrieving permission with ID {id} from Elasticsearch: {response.OriginalException.Message}");
            }

            return response.Source;
        }
    }
}