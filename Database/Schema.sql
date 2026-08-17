/* =========================================================
   SudsLog Car Wash — Database Setup Script
   Run this in SQL Server Management Studio (SSMS):
     1. Open SSMS, connect to your server instance.
     2. Open a New Query window.
     3. Paste this whole script and click Execute (F5).
   This matches the EF Core models exactly, so the ASP.NET
   app can connect straight to it with no migrations needed.
   ========================================================= */

IF DB_ID('CarWashDB') IS NULL
BEGIN
    CREATE DATABASE CarWashDB;
END
GO

USE CarWashDB;
GO

-- ---------------------------------------------------------
-- Employees
-- ---------------------------------------------------------
IF OBJECT_ID('dbo.Employees', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Employees (
        EmployeeNum   INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeName  NVARCHAR(100)  NOT NULL,
        ServiceType   NVARCHAR(50)   NOT NULL,
        Phone         NVARCHAR(20)   NULL,
        Status        NVARCHAR(20)   NOT NULL DEFAULT ('Active')
    );
END
GO

-- ---------------------------------------------------------
-- Clients
-- ---------------------------------------------------------
IF OBJECT_ID('dbo.Clients', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Clients (
        ClientId      INT IDENTITY(1,1) PRIMARY KEY,
        ClientName    NVARCHAR(100)  NOT NULL,
        Phone         NVARCHAR(20)   NULL,
        Email         NVARCHAR(100)  NULL,
        LicensePlate  NVARCHAR(20)   NULL,
        Notes         NVARCHAR(500)  NULL
    );
END
GO

-- ---------------------------------------------------------
-- Washes  (the daily wash log: CarID, CarName, WashTime, Employee)
-- ---------------------------------------------------------
IF OBJECT_ID('dbo.Washes', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Washes (
        CarID         INT IDENTITY(1,1) PRIMARY KEY,
        CarName       NVARCHAR(100)   NOT NULL,
        LicensePlate  NVARCHAR(20)    NULL,
        WashTime      TIME            NOT NULL,
        WashDate      DATE            NOT NULL DEFAULT (CONVERT(date, GETDATE())),
        ServiceType   NVARCHAR(50)    NULL,
        Price         DECIMAL(8,2)    NOT NULL DEFAULT (0),
        Status        NVARCHAR(20)    NOT NULL DEFAULT ('In Progress'),
        EmployeeNum   INT             NULL,
        ClientId      INT             NULL,

        CONSTRAINT FK_Washes_Employees FOREIGN KEY (EmployeeNum)
            REFERENCES dbo.Employees(EmployeeNum) ON DELETE SET NULL,

        CONSTRAINT FK_Washes_Clients FOREIGN KEY (ClientId)
            REFERENCES dbo.Clients(ClientId) ON DELETE SET NULL
    );
END
GO

-- ---------------------------------------------------------
-- Helpful indexes for daily lookups
-- ---------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Washes_WashDate')
    CREATE INDEX IX_Washes_WashDate ON dbo.Washes(WashDate);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Washes_Status')
    CREATE INDEX IX_Washes_Status ON dbo.Washes(Status);
GO

-- ---------------------------------------------------------
-- Sample seed data (safe to delete once you're up and running)
-- ---------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Employees)
BEGIN
    INSERT INTO dbo.Employees (EmployeeName, ServiceType, Phone, Status) VALUES
        (N'Sipho Ndlovu',   N'Full Valet',      N'083 555 0192', N'Active'),
        (N'Ayanda Khumalo', N'Exterior Wash',   N'072 555 3321', N'Active'),
        (N'Given Mokoena',  N'Interior Detail', N'061 555 7743', N'Off Shift');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Clients)
BEGIN
    INSERT INTO dbo.Clients (ClientName, Phone, Email, LicensePlate, Notes) VALUES
        (N'Nomvula Zulu',  N'072 555 8834', N'nomvula@email.com', N'ND 45 67 GP', N'Prefers wax finish'),
        (N'Trevor Naidoo', N'082 555 1120', N'trevor.n@email.com', N'ND 88 21 GP', NULL);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Washes)
BEGIN
    INSERT INTO dbo.Washes (CarName, LicensePlate, WashTime, WashDate, ServiceType, Price, Status, EmployeeNum, ClientId) VALUES
        (N'Toyota Corolla', N'ND 45 67 GP', '09:30', CONVERT(date, GETDATE()), N'Wax & Polish',   180.00, N'In Progress', 1, 1),
        (N'VW Polo',        N'ND 88 21 GP', '10:15', CONVERT(date, GETDATE()), N'Exterior Wash',   80.00, N'Waiting',     2, 2),
        (N'Ford Ranger',    NULL,           '08:45', CONVERT(date, GETDATE()), N'Full Valet',     260.00, N'Done',        1, NULL);
END
GO

PRINT 'CarWashDB is ready.';
