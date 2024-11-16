using ChallengerTest.Models;

namespace ChallengerTest.Repositories.Command
{
    public interface IPermissionCommandRepository
    {
        Task AddPermissionAsync(Permission permission);  // Agregar un permiso a la base de datos
        Task SaveAsync();  // Guardar cambios en la base de datos
        Task IndexPermissionAsync(Permission permission);  // Indexar en Elasticsearch
        Task UpdatePermissionAsync(Permission permission);  // Método para actualizar un permiso
    }
}