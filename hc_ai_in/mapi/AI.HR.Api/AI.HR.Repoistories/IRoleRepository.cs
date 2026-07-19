using AI.HR.Models;

namespace AI.HR.Repoistories;

/// <summary>
/// Data read operations for roles.
/// </summary>
public interface IRoleRepository
{
    /// <summary>Returns all roles.</summary>
    Task<List<RoleItem>> GetAll();
}
