using ChallengerTest.Models;
using ChallengerTest.Repositories.Command;
using ChallengerTest.Repositories.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;

namespace ChallengerTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionQueryRepository _permissionQueryRepository;
        private readonly IPermissionCommandRepository _permissionCommandRepository;
        private readonly ILogger<PermissionsController> _logger;

        public PermissionsController(
            IPermissionQueryRepository permissionQueryRepository,
            IPermissionCommandRepository permissionCommandRepository,
            ILogger<PermissionsController> logger)
        {
            _permissionQueryRepository = permissionQueryRepository;
            _permissionCommandRepository = permissionCommandRepository;
            _logger = logger;
        }

        // GET: api/permissions/get
        [HttpGet("get")]
        public async Task<IActionResult> GetPermissions()
        {
            try
            {
                _logger.LogInformation("Inicio de Get Permissions...");
                var permissions = await _permissionQueryRepository.GetPermissionsAsync();
                _logger.LogInformation("Fin de Get Permissions...");
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener datos de los Permissions.");
                return StatusCode(500, $"Error retrieving permissions: {ex.Message}");
            }
        }

        // POST: api/permissions/request
        [HttpPost("request")]
        public async Task<IActionResult> RequestPermission([FromBody] Permission permission)
        {
            try
            {
                _logger.LogInformation("Inicio de Request Permission...");
                // Se guarda en la base de datos
                await _permissionCommandRepository.AddPermissionAsync(permission);
                await _permissionCommandRepository.SaveAsync();

                // Se indexa en Elasticsearch
                await _permissionCommandRepository.IndexPermissionAsync(permission);

                _logger.LogInformation("Fin de RequestPermission.");
                return Ok("Permission requested successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en RequestPermission.");
                return StatusCode(500, $"Error processing permission request: {ex.Message}");
            }
        }

        // PUT: api/permissions/modify/{id}
        [HttpPut("modify/{id}")]
        public async Task<IActionResult> ModifyPermission(int id, [FromBody] Permission updatedPermission)
        {
            try
            {
                _logger.LogInformation("Inicio de Modify Permission con id: {Id}", id);
                // Verificar si el registro del id existe
                var existingPermission = await _permissionQueryRepository.GetPermissionByIdAsync(id);
                if (existingPermission == null)
                {
                    _logger.LogInformation("Permission con ID {ID} not found.", id);
                    return NotFound($"Permission with ID {id} not found.");
                }

                // Actualizar el registro con el id solicitado con los nuevos datos
                existingPermission.EmployeeForename = updatedPermission.EmployeeForename;
                existingPermission.EmployeeSurname = updatedPermission.EmployeeSurname;
                existingPermission.PermissionType = updatedPermission.PermissionType;
                existingPermission.PermissionDate = updatedPermission.PermissionDate;

                // Actualiza en la base de datos y en Elasticsearch
                await _permissionCommandRepository.UpdatePermissionAsync(existingPermission);

                _logger.LogInformation("Fin de Modify Permission.");
                return Ok("Permission updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ModifyPermission.");
                return StatusCode(500, $"Error updating permission: {ex.Message}");
            }
        }
    }
}