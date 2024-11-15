using ChallengerTest.Models;
using Nest;

namespace ChallengerTest.Services
{
    public interface IElasticsearchService
    {
        Task IndexPermissionAsync(Permission permission);
    }

    public class ElasticsearchService : IElasticsearchService
    {
        private readonly IElasticClient _elasticClient;

        public ElasticsearchService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task IndexPermissionAsync(Permission permission)
        {
            var response = await _elasticClient.IndexDocumentAsync(permission);
            if (!response.IsValid)
            {
                throw new Exception("Error indexing permission in Elasticsearch.");
            }
        }
    }
}