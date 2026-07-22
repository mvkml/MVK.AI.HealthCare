using HC.AI.Identity.EF.DBContexts;
using HC.AI.Identity.Models;
using EfRole = HC.AI.Identity.EF.Entities.Role;
using Microsoft.EntityFrameworkCore;

namespace HC.AI.Identity.Repositories;

/// <summary>
/// Data read operations for roles, backed by AiHrDbContext.
/// </summary>
public class RoleRepository : IRoleRepository
{
    private readonly AiHrDbContext _context;

    /// <summary>Creates the repository with the given DbContext.</summary>
    public RoleRepository(AiHrDbContext context)
    {
        _context = context;
    }

    /// <summary>Returns all roles, ordered by OrderId.</summary>
    public async Task<List<RoleItem>> GetAll()
    {
        var entities = await _context.Roles.OrderBy(r => r.OrderId).ToListAsync();
        return entities.Select(ToModel).ToList();
    }

    private static RoleItem ToModel(EfRole entity) => new()
    {
        RoleId = entity.RoleId,
        RoleName = entity.RoleName,
        OrderId = entity.OrderId,
    };
}
