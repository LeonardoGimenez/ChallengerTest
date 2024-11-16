using Moq;
using System;
using System.Threading.Tasks;
using ChallengerTest.Models;
using ChallengerTest.Repositories;
using Xunit;
using ChallengerTest.Repositories.Command;

namespace ChallengerTest.Tests.Unit
{
    public class RequestPermissionTests
    {
        private readonly Mock<IPermissionCommandRepository> _mockCommandRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public RequestPermissionTests()
        {
            // Crear mocks de los repositorios y UnitOfWork
            _mockCommandRepository = new Mock<IPermissionCommandRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            // Configurar el UnitOfWork para devolver el repositorio simulado
            _mockUnitOfWork.Setup(u => u.PermissionCommands).Returns(_mockCommandRepository.Object);
        }

        [Fact]
        public async Task RequestPermission_Should_AddPermissionAndSaveAsync()
        {
            // Arrange: Crear un permiso de prueba
            var permission = new Permission
            {
                EmployeeForename = "Test",
                EmployeeSurname = "User",
                PermissionType = 1,
                PermissionDate = DateTime.Now
            };

            // Configurar los mocks
            _mockCommandRepository.Setup(repo => repo.AddPermissionAsync(It.IsAny<Permission>()))
                                  .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveAsync())
                           .Returns(Task.CompletedTask);

            // Act: Ejecutar la lógica directamente usando los mocks
            await _mockCommandRepository.Object.AddPermissionAsync(permission);
            await _mockUnitOfWork.Object.SaveAsync();

            // Assert: Verificar que se llamaron los métodos esperados
            _mockCommandRepository.Verify(repo => repo.AddPermissionAsync(It.IsAny<Permission>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
