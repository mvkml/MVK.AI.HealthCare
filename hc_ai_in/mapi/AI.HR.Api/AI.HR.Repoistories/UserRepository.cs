using AI.HR.EF.DBContexts;
using AI.HR.Models;
using EfUser = AI.HR.EF.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace AI.HR.Repoistories;

/// <summary>
/// Data persistence operations for users, backed by AiHrDbContext.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AiHrDbContext _context;

    /// <summary>Creates the repository with the given DbContext.</summary>
    public UserRepository(AiHrDbContext context)
    {
        _context = context;
    }

    /// <summary>Returns the user with the given Email, or null if not found.</summary>
    public async Task<UserItem?> GetByEmail(string email)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return entity is null ? null : ToModel(entity);
    }

    /// <summary>Inserts a new user and returns the persisted UserItem.</summary>
    public async Task<UserItem> Create(UserItem userItem)
    {
        var entity = ToEntity(userItem);
        _context.Users.Add(entity);
        await _context.SaveChangesAsync();
        return ToModel(entity);
    }

    /// <summary>Updates an existing user by Email and returns the persisted UserItem, or null if not found.</summary>
    public async Task<UserItem?> Update(UserItem userItem)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(u => u.Email == userItem.Email);
        if (entity is null)
        {
            return null;
        }

        entity.FullName = userItem.FullName;
        entity.Company = userItem.Company;
        entity.PasswordHash = userItem.PasswordHash;
        entity.IsActive = userItem.IsActive;
        entity.RoleId = userItem.RoleId;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ToModel(entity);
    }

    /// <summary>Deletes a user by Email. Returns true if a user was deleted.</summary>
    public async Task<bool> Delete(string email)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (entity is null)
        {
            return false;
        }

        _context.Users.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>Creates the user if the Email doesn't exist yet, otherwise updates it.</summary>
    public async Task<UserItem> Upsert(UserItem userItem)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == userItem.Email);
        if (!exists)
        {
            return await Create(userItem);
        }

        var updated = await Update(userItem);
        return updated!;
    }

    /// <summary>Updates only the PasswordHash for the user with the given Email. Returns true if a user was found and updated.</summary>
    public async Task<bool> UpdatePassword(string email, string passwordHash)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (entity is null)
        {
            return false;
        }

        entity.PasswordHash = passwordHash;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private static EfUser ToEntity(UserItem userItem) => new()
    {
        FullName = userItem.FullName,
        Email = userItem.Email,
        Company = userItem.Company,
        PasswordHash = userItem.PasswordHash,
        CreatedAt = userItem.CreatedAt,
        UpdatedAt = userItem.UpdatedAt,
        IsActive = userItem.IsActive,
        RoleId = userItem.RoleId,
    };

    private static UserItem ToModel(EfUser entity) => new()
    {
        UserId = entity.UserId,
        FullName = entity.FullName,
        Email = entity.Email,
        Company = entity.Company,
        PasswordHash = entity.PasswordHash,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        IsActive = entity.IsActive,
        RoleId = entity.RoleId,
    };
}
