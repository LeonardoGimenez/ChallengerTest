using ChallengerTest.Models;

namespace ChallengerTest.Repositories.Query
{
    public interface IPermissionQueryRepository
    {
        Task<List<Permission>> GetPermissionsAsync();  // Obtener todos los permisos
        Task<Permission> GetPermissionByIdAsync(int id);  // Obtener un permiso por ID
    }
}