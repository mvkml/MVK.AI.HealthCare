using HC.AI.Admin.EF.DBContexts;
using HC.AI.Admin.Models;
using EfAdmin = HC.AI.Admin.EF.Entities.AdminAccount;
using Microsoft.EntityFrameworkCore;

namespace HC.AI.Admin.Repositories;

/// <summary>
/// Data persistence operations for admins, backed by AdminDbContext.
/// </summary>
public class AdminRepository : IAdminRepository
{
    private readonly AdminDbContext _context;

    /// <summary>Creates the repository with the given DbContext.</summary>
    public AdminRepository(AdminDbContext context)
    {
        _context = context;
    }

    /// <summary>Returns the admin with the given Email, or null if not found.</summary>
    public async Task<AdminItem?> GetByEmail(string email)
    {
        var entity = await _context.Admins.FirstOrDefaultAsync(a => a.Email == email);
        return entity is null ? null : ToModel(entity);
    }

    /// <summary>Returns true if an admin with the given Email already exists.</summary>
    public async Task<bool> ExistsByEmail(string email)
    {
        return await _context.Admins.AnyAsync(a => a.Email == email);
    }

    /// <summary>Inserts a new admin and returns the persisted AdminItem.</summary>
    public async Task<AdminItem> Create(AdminItem adminItem)
    {
        var entity = ToEntity(adminItem);
        _context.Admins.Add(entity);
        await _context.SaveChangesAsync();
        return ToModel(entity);
    }

    private static EfAdmin ToEntity(AdminItem adminItem) => new()
    {
        FullName = adminItem.FullName,
        Email = adminItem.Email,
        PasswordHash = adminItem.PasswordHash,
        CreatedAt = adminItem.CreatedAt,
        UpdatedAt = adminItem.UpdatedAt,
        IsActive = adminItem.IsActive,
    };

    private static AdminItem ToModel(EfAdmin entity) => new()
    {
        AdminId = entity.AdminId,
        FullName = entity.FullName,
        Email = entity.Email,
        PasswordHash = entity.PasswordHash,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        IsActive = entity.IsActive,
    };
}
