using ChallengerTest.Models;
using ChallengerTest.Repositories;
using ChallengerTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IElasticsearchService _elasticsearchService;

        public PermissionsController(IUnitOfWork unitOfWork, IElasticsearchService elasticsearchService)
        {
            _unitOfWork = unitOfWork;
            _elasticsearchService = elasticsearchService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestPermission([FromBody] Permission permission)
        {
            if (permission == null)
            {
                return BadRequest("Invalid data.");
            }

            // Persistir en SQL Server
            await _unitOfWork.Permissions.AddPermissionAsync(permission);
            await _unitOfWork.SaveAsync();

            // Persistir en Elasticsearch
            await _elasticsearchService.IndexPermissionAsync(permission);

            return Ok(permission);
        }
    }
}