USE [testChallenger];
GO

-- Crear la tabla PermissionTypes
CREATE TABLE PermissionTypes (
    Id INT IDENTITY(1,1) PRIMARY KEY,  
    Description NVARCHAR(255) NOT NULL 
);
GO

-- Crear la tabla Permissions
CREATE TABLE Permissions (
    Id INT IDENTITY(1,1) PRIMARY KEY,       
    EmployeeForename NVARCHAR(100) NOT NULL, 
    EmployeeSurname NVARCHAR(100) NOT NULL, 
    PermissionType INT NOT NULL,            
    PermissionDate DATE NOT NULL,          
    CONSTRAINT FK_Permissions_PermissionTypes FOREIGN KEY (PermissionType)
    REFERENCES PermissionTypes (Id)         
    ON DELETE CASCADE                      
    ON UPDATE CASCADE                       
);
GO
