using ChallengerTest.Models;
using ChallengerTest.Repositories;
using ChallengerTest.Repositories.Command;
using ChallengerTest.Repositories.Query;

using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ChallengerTest.Tests.Unit
{
    public class PermissionsServiceTests
    {
        private readonly Mock<IPermissionCommandRepository> _mockCommandRepository;
        private readonly Mock<IPermissionQueryRepository> _mockQueryRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public PermissionsServiceTests()
        {
            _mockCommandRepository = new Mock<IPermissionCommandRepository>();
            _mockQueryRepository = new Mock<IPermissionQueryRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            // Configurar el UnitOfWork para devolver el repositorio simulado
            _mockUnitOfWork.Setup(u => u.PermissionCommands).Returns(_mockCommandRepository.Object);
            _mockUnitOfWork.Setup(u => u.PermissionQueries).Returns(_mockQueryRepository.Object);
        }

        // Test for RequestPermission
        [Fact]
        public async Task RequestPermission_Should_AddPermissionAndSaveAsync()
        {
            // Arrange
            var permission = new Permission
            {
                EmployeeForename = "Test",
                EmployeeSurname = "User",
                PermissionType = 1,
                PermissionDate = DateTime.Now
            };

            _mockUnitOfWork
                .Setup(u => u.PermissionCommands.AddPermissionAsync(permission))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(u => u.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _mockUnitOfWork.Object.PermissionCommands.AddPermissionAsync(permission);
            await _mockUnitOfWork.Object.SaveAsync();

            // Assert
            _mockUnitOfWork.Verify(u => u.PermissionCommands.AddPermissionAsync(permission), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        // Test for ModifyPermission
        [Fact]
        public async Task ModifyPermission_Should_UpdatePermissionAndSaveAsync()
        {
            // Arrange
            var permissionId = 1;
            var existingPermission = new Permission
            {
                Id = permissionId,
                EmployeeForename = "Original",
                EmployeeSurname = "User",
                PermissionType = 1,
                PermissionDate = DateTime.Now.AddDays(-1)
            };

            var updatedPermission = new Permission
            {
                Id = permissionId,
                EmployeeForename = "Updated",
                EmployeeSurname = "User",
                PermissionType = 2,
                PermissionDate = DateTime.Now
            };

            _mockUnitOfWork
                .Setup(u => u.PermissionQueries.GetPermissionByIdAsync(permissionId))
                .ReturnsAsync(existingPermission);

            _mockUnitOfWork
                .Setup(u => u.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var permissionToModify = await _mockUnitOfWork.Object.PermissionQueries.GetPermissionByIdAsync(permissionId);

            if (permissionToModify != null)
            {
                permissionToModify.EmployeeForename = updatedPermission.EmployeeForename;
                permissionToModify.EmployeeSurname = updatedPermission.EmployeeSurname;
                permissionToModify.PermissionType = updatedPermission.PermissionType;
                permissionToModify.PermissionDate = updatedPermission.PermissionDate;

                await _mockUnitOfWork.Object.SaveAsync();
            }

            // Assert
            _mockUnitOfWork.Verify(u => u.PermissionQueries.GetPermissionByIdAsync(permissionId), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);

            Assert.Equal(updatedPermission.EmployeeForename, existingPermission.EmployeeForename);
            Assert.Equal(updatedPermission.EmployeeSurname, existingPermission.EmployeeSurname);
            Assert.Equal(updatedPermission.PermissionType, existingPermission.PermissionType);
            Assert.Equal(updatedPermission.PermissionDate, existingPermission.PermissionDate);
        }

        //Test for GetPermissions
        [Fact]
        public async Task GetPermissions_Should_ReturnAllPermissions()
        {
            // Arrange
            var permissionsList = new List<Permission>
            {
                new Permission { Id = 4, EmployeeForename = "Mick", EmployeeSurname = "Jagger", PermissionType = 1 },
                new Permission { Id = 5, EmployeeForename = "Robert", EmployeeSurname = "Plant", PermissionType = 1 }
            };

            _mockUnitOfWork
                .Setup(u => u.PermissionQueries.GetPermissionsAsync())
                .ReturnsAsync(permissionsList);

            // Act
            var result = await _mockUnitOfWork.Object.PermissionQueries.GetPermissionsAsync();

            // Assert
            _mockUnitOfWork.Verify(u => u.PermissionQueries.GetPermissionsAsync(), Times.Once);
            Assert.Equal(permissionsList.Count, result.Count);
            Assert.Contains(result, p => p.EmployeeForename == "Mick");
            Assert.Contains(result, p => p.EmployeeForename == "Robert");
        }
    }
}
