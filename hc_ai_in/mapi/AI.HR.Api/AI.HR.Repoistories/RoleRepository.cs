using AI.HR.EF.DBContexts;
using AI.HR.Models;
using EfRole = AI.HR.EF.Entities.Role;
using Microsoft.EntityFrameworkCore;

namespace AI.HR.Repoistories;

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
