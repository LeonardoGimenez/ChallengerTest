using ChallengerTest.Models;
using ChallengerTest.Repositories.Query;
using Nest;
using Moq;
using Microsoft.Extensions.Logging;
using Xunit;
using ChallengerTest.Controllers;
using ChallengerTest.Data;
using ChallengerTest.Repositories.Command;
using ChallengerTest.Repositories.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChallengerTest.Repositories;

namespace ChallengerTest.Tests.Integration
{
    public class PermissionsControllerIntegrationTests
    {
        private readonly ApplicationDbContext _context;
        private readonly PermissionsController _controller;

        public PermissionsControllerIntegrationTests()
        {
            // Configurar la base de datos en memoria
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("PermissionsTestDb")
                .Options;

            _context = new ApplicationDbContext(options);

            // Crear un mock de IElasticClient
            var mockElasticClient = new Mock<IElasticClient>();

            // Crear repositorios reales (pasando el contexto y el cliente de Elasticsearch)
            var commandRepository = new PermissionCommandRepository(_context, mockElasticClient.Object);
            var queryRepository = new PermissionQueryRepository(mockElasticClient.Object);  // Usamos el cliente simulado

            // Configurar dependencias adicionales
            var mockLogger = new Mock<ILogger<PermissionsController>>();
            var mockKafkaProducer = new Mock<KafkaProducer>(); // Kafka simulado

            // Crear instancia del controlador
            _controller = new PermissionsController(
                queryRepository,
                commandRepository,
                mockLogger.Object,
                mockKafkaProducer.Object
            );
        }

        [Fact]
        public async Task RequestPermission_Should_AddPermissionSuccessfully()
        {
            // Arrange
            var permission = new Permission
            {
                EmployeeForename = "John",
                EmployeeSurname = "Doe",
                PermissionType = 1,
                PermissionDate = DateTime.Now
            };

            // Añadir el contexto en memoria para usar en la prueba
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            // Crear el mock para el PermissionCommandRepository
            var mockCommandRepository = new Mock<IPermissionCommandRepository>();
            mockCommandRepository.Setup(repo => repo.AddPermissionAsync(It.IsAny<Permission>()))
                                 .Returns(Task.CompletedTask);

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(u => u.PermissionCommands).Returns(mockCommandRepository.Object);
            mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            // Crear el mock para el PermissionQueryRepository ( no se usará directamente en este test)
            var mockQueryRepository = new Mock<IPermissionQueryRepository>();

            // Crear el mock de KafkaProducer y Logger
            var kafkaProducerMock = new Mock<KafkaProducer>();
            var loggerMock = new Mock<ILogger<PermissionsController>>();

            // Crear el controlador con la inyección de dependencias
            var controller = new PermissionsController(
                mockQueryRepository.Object,    // Inyectar el mock de IPermissionQueryRepository
                mockCommandRepository.Object,  // Inyectar el mock de IPermissionCommandRepository
                loggerMock.Object,             // Inyectar el mock de ILogger<PermissionsController>
                kafkaProducerMock.Object       // Inyectar el mock de KafkaProducer
            );

            // Act: Ejecutar la acción RequestPermission con el permiso creado
            var result = await controller.RequestPermission(permission);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);  // Verifica que la respuesta sea OkObjectResult
            Assert.Equal("Permission requested successfully", okResult.Value);  // Verifica el mensaje de éxito
        }

        [Fact]
        public async Task GetPermissions_Should_ReturnAllPermissions()
        {
            // Arrange: Creación de los permisos en la base de datos en memoria
            var permissions = new List<Permission>
            {
                new Permission { Id = 2, EmployeeForename = "John", EmployeeSurname = "Doe", PermissionType = 1, PermissionDate = DateTime.Now },
                new Permission { Id = 3, EmployeeForename = "Jane", EmployeeSurname = "Smith", PermissionType = 2, PermissionDate = DateTime.Now.AddDays(1) }
            };

            foreach (var permission in permissions)
            {
                _context.Permissions.Add(permission);
            }
            await _context.SaveChangesAsync();

            // Crear los mocks necesarios
            var mockCommandRepository = new Mock<IPermissionCommandRepository>();
            var mockQueryRepository = new Mock<IPermissionQueryRepository>();
            mockQueryRepository.Setup(repo => repo.GetPermissionsAsync()).ReturnsAsync(permissions);
            var kafkaProducerMock = new Mock<KafkaProducer>();
            var loggerMock = new Mock<ILogger<PermissionsController>>();

            // Crear el controlador con la inyección de dependencias
            var controller = new PermissionsController(
                mockQueryRepository.Object,
                mockCommandRepository.Object,
                loggerMock.Object,
                kafkaProducerMock.Object
            );

            // Act: Ejecutar la acción GetPermissions
            var result = await controller.GetPermissions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); // Verificar que la respuesta sea OkObjectResult
            var returnValue = Assert.IsType<List<Permission>>(okResult.Value); // Verificar que el valor retornado sea de tipo List<Permission>
            Assert.Equal(2, returnValue.Count); // Verificar que el número de permisos sea 2
        }

        [Fact]
        public async Task ModifyPermission_Should_UpdatePermissionSuccessfully()
        {
            // Arrange
            var existingPermission = new Permission
            {
                Id = 4,
                EmployeeForename = "John",
                EmployeeSurname = "Doe",
                PermissionType = 1,
                PermissionDate = DateTime.Now
            };

            var updatedPermission = new Permission
            {
                Id = 4,
                EmployeeForename = "John",
                EmployeeSurname = "Doe Updated",
                PermissionType = 2,
                PermissionDate = DateTime.Now.AddDays(1)
            };

            // Guardamos el permiso inicial en la base de datos en memoria
            _context.Permissions.Add(existingPermission);
            await _context.SaveChangesAsync();

            // Simulamos la actualización en el repositorio
            var mockCommandRepository = new Mock<IPermissionCommandRepository>();
            var mockQueryRepository = new Mock<IPermissionQueryRepository>();
            mockQueryRepository.Setup(repo => repo.GetPermissionByIdAsync(existingPermission.Id)).ReturnsAsync(existingPermission);

            // Configuración de los mocks restantes
            var kafkaProducerMock = new Mock<KafkaProducer>();
            var loggerMock = new Mock<ILogger<PermissionsController>>();

            // Creamos el controlador
            var controller = new PermissionsController(
                mockQueryRepository.Object,
                mockCommandRepository.Object,
                loggerMock.Object,
                kafkaProducerMock.Object
            );

            // Act
            var result = await controller.ModifyPermission(existingPermission.Id, updatedPermission);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Permission updated successfully", okResult.Value);

            // Verificar que el permiso fue actualizado en la base de datos
            var permissionInDb = await _context.Permissions.FindAsync(existingPermission.Id);
            Assert.NotNull(permissionInDb);
            Assert.Equal(updatedPermission.EmployeeForename, permissionInDb.EmployeeForename);
            Assert.Equal(updatedPermission.EmployeeSurname, permissionInDb.EmployeeSurname);
            Assert.Equal(updatedPermission.PermissionType, permissionInDb.PermissionType);
            Assert.Equal(updatedPermission.PermissionDate, permissionInDb.PermissionDate);
        }
    }
}
