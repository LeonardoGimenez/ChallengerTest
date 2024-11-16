using ChallengerTest.Models;
using ChallengerTest.Repositories.Command;
using ChallengerTest.Repositories.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChallengerTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionQueryRepository _permissionQueryRepository;
        private readonly IPermissionCommandRepository _permissionCommandRepository;

        public PermissionsController(
            IPermissionQueryRepository permissionQueryRepository,
            IPermissionCommandRepository permissionCommandRepository)
        {
            _permissionQueryRepository = permissionQueryRepository;
            _permissionCommandRepository = permissionCommandRepository;
        }

        // GET: api/permissions/get
        [HttpGet("get")]
        public async Task<IActionResult> GetPermissions()
        {
            try
            {
                var permissions = await _permissionQueryRepository.GetPermissionsAsync();
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving permissions: {ex.Message}");
            }
        }

        // POST: api/permissions/request
        [HttpPost("request")]
        public async Task<IActionResult> RequestPermission([FromBody] Permission permission)
        {
            try
            {
                // Se guarda en la base de datos
                await _permissionCommandRepository.AddPermissionAsync(permission);
                await _permissionCommandRepository.SaveAsync();

                // Se indexa en Elasticsearch
                await _permissionCommandRepository.IndexPermissionAsync(permission);

                return Ok("Permission requested successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing permission request: {ex.Message}");
            }
        }
    }
}