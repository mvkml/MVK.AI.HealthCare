-- PB023 support tables: authentication (Users/Roles) and document-intelligence tracking
-- (OcrDocuments), incorporated into AI_HealthCarePatient from the previously separate AI_HR
-- database/solution (hc_apis/az/hc_core_apis/HC.AI.Identity.Api, renamed 2026-07-19 from AI.HR.Api).
-- Schema is EF Core-managed by HC.AI.Identity.EF.DBContexts.AiHrDbContext (class name unchanged
-- by the rename) — this script documents the resulting shape for the Dev SQL Agent's owned
-- reference; actual DDL for this project's copy was applied via `dotnet ef database update`
-- (HC.AI.Identity.EF migrations), not run directly.
--
-- Source database (AI_HR) is left untouched per user instruction — this is an additive copy,
-- not a migration/cutover of the original database.

CREATE TABLE dbo.Roles (
    RoleId    INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
    RoleName  NVARCHAR(100) NOT NULL,
    OrderId   INT           NOT NULL,
    CONSTRAINT UQ_Roles_RoleName UNIQUE (RoleName)
);
GO

CREATE TABLE dbo.Users (
    UserId       INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
    FullName     NVARCHAR(150) NOT NULL,
    Email        NVARCHAR(255) NOT NULL,
    Company      NVARCHAR(150) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt    DATETIME2     NULL,
    IsActive     BIT           NOT NULL DEFAULT 1,
    RoleId       INT           NOT NULL,
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles (RoleId)
);
GO

CREATE TABLE dbo.OcrDocuments (
    Id               INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
    DocumentNumber   NVARCHAR(50)   NOT NULL,
    GroupNumber      NVARCHAR(50)   NULL,
    FileName         NVARCHAR(255)  NOT NULL,
    FileExtension    NVARCHAR(10)   NOT NULL,
    DocumentType     NVARCHAR(50)   NOT NULL,
    SourceLocation   NVARCHAR(500)  NULL,
    BlobUrl          NVARCHAR(1000) NULL,
    BlobSize         BIGINT         NULL,
    DfInstanceId     NVARCHAR(100)  NULL,
    Status           NVARCHAR(20)   NOT NULL DEFAULT 'Pending',
    IsActive         BIT            NOT NULL DEFAULT 1,
    CreatedDateTime  DATETIME2      NOT NULL,
    UpdatedDateTime  DATETIME2      NULL,
    CONSTRAINT UQ_OcrDocuments_DocumentNumber UNIQUE (DocumentNumber)
);
GO

-- Seeded roles (via AiHrDbContext.OnModelCreating HasData), simplified to this project's persona
-- model per user instruction (2026-07-19): 1 Doctor, 2 Patient. The original HR-domain roles
-- (HR Manager, HR Executive, Recruiter, Payroll Manager, Team Lead, Developer, Other) were removed
-- via migration `SimplifyRolesToDoctorPatient`; existing Users referencing the removed RoleIds
-- (3-7) were reassigned to Patient (2) first so the migration's DELETE didn't violate the
-- Users->Roles FK. User explicitly said this is a starting point ("later we will change
-- accordingly") — not necessarily the final persona model.
