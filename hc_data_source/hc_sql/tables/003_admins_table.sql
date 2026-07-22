-- PB033 support table: Admin authentication (HC.AI.Admin.API), added to AI_HealthCarePatient
-- (one-database convention, same as HC.AI.Identity.Api's Users/Roles/OcrDocuments). Schema is EF
-- Core-managed by HC.AI.Admin.EF.DBContexts.AdminDbContext -- this script documents the resulting
-- shape for the Dev SQL Agent's owned reference; actual DDL was applied via
-- `dotnet ef database update` (HC.AI.Admin.EF migrations), not run directly.
--
-- Deliberately a separate table from Users, not a shared table with a role flag -- Admin is a
-- genuinely separate persona/system, and this table carries none of the HR-domain leftover
-- fields (Company, RoleId) that Users inherited from the AI.HR.Api origin.

CREATE TABLE dbo.Admins (
    AdminId      INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
    FullName     NVARCHAR(150) NOT NULL,
    Email        NVARCHAR(255) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt    DATETIME2     NULL,
    IsActive     BIT           NOT NULL DEFAULT 1,
    CONSTRAINT UQ_Admins_Email UNIQUE (Email)
);
GO
