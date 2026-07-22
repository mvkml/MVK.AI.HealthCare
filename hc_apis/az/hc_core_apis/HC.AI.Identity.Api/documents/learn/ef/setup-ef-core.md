# Setting up Entity Framework Core — AI.HR.EF

Context: `AI.HR.EF` is currently a bare class library (`net8.0`) with a placeholder `Class1.cs`. This project will hold the EF Core `DbContext` and entity models for the `AI_HR` database on `(localdb)\MSSQLLocalDB`.

## 0. Check SDK/tooling versions

```powershell
dotnet --list-sdks
```

Both `8.0.x` and `9.0.x` may be installed. Confirm which SDK the solution should build against (matches `<TargetFramework>` in the `.csproj` files) before installing packages, to avoid version mismatches between the SDK and EF Core tooling.

## 1. Install the EF Core CLI tool (once per machine)

```powershell
dotnet tool install --global dotnet-ef
# or, if already installed:
dotnet tool update --global dotnet-ef
```

## 2. Add NuGet packages to AI.HR.EF

```powershell
cd C:\git\v\ai_hr\hr_apis\az\hr_core_apis\AI.HR.Api\AI.HR.EF
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

Use package versions that match the project's target framework (e.g. EF Core 8.x for `net8.0`).

## 3. Define entity classes

Replace `Class1.cs` with one class per table, e.g.:

```csharp
public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
```

## 4. Create the DbContext

```csharp
using Microsoft.EntityFrameworkCore;

public class AiHrDbContext : DbContext
{
    public AiHrDbContext(DbContextOptions<AiHrDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
}
```

## 5. Reference AI.HR.EF from AI.HR.Api

```powershell
cd C:\git\v\ai_hr\hr_apis\az\hr_core_apis\AI.HR.Api\AI.HR.Api
dotnet add reference ..\AI.HR.EF\AI.HR.EF.csproj
```

## 6. Register the DbContext in Program.cs

```csharp
builder.Services.AddDbContext<AiHrDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AiHrDb")));
```

Add the connection string to `appsettings.json`:

```json
"ConnectionStrings": {
  "AiHrDb": "Server=(localdb)\\MSSQLLocalDB;Database=AI_HR;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

## 7. Create the initial migration

```powershell
dotnet ef migrations add InitialCreate --project AI.HR.EF --startup-project AI.HR.Api
```

## 8. Apply the migration to the database

```powershell
dotnet ef database update --project AI.HR.EF --startup-project AI.HR.Api
```

## 9. Verify

Connect to `(localdb)\MSSQLLocalDB` and confirm the `AI_HR` database now has the expected tables (e.g. via SSMS, Azure Data Studio, or `sqlcmd`).

## Troubleshooting

- **SDK/tooling mismatch**: if `dotnet ef` errors reference an unexpected runtime version, check that the global `dotnet-ef` tool version and the project's installed EF Core package versions both support the project's `TargetFramework`. Pin the repo to a specific SDK with a `global.json` if multiple SDKs are installed and the wrong one is being picked up.
- **Design-time DbContext not found**: ensure `--startup-project AI.HR.Api` is passed, since `AI.HR.EF` itself isn't an executable project.
