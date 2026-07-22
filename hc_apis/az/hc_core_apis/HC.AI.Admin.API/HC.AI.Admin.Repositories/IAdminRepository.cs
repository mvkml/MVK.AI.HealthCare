using HC.AI.Admin.Models;

namespace HC.AI.Admin.Repositories;

/// <summary>
/// Data persistence operations for admins.
/// </summary>
public interface IAdminRepository
{
    /// <summary>Returns the admin with the given Email, or null if not found.</summary>
    Task<AdminItem?> GetByEmail(string email);

    /// <summary>Returns true if an admin with the given Email already exists.</summary>
    Task<bool> ExistsByEmail(string email);

    /// <summary>Inserts a new admin and returns the persisted AdminItem.</summary>
    Task<AdminItem> Create(AdminItem adminItem);
}
