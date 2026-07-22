using HC.AI.Identity.Models;

namespace HC.AI.Identity.Repositories;

/// <summary>
/// Data read operations for roles.
/// </summary>
public interface IRoleRepository
{
    /// <summary>Returns all roles.</summary>
    Task<List<RoleItem>> GetAll();
}
