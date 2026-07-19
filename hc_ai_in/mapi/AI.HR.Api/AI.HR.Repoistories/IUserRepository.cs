using AI.HR.Models;

namespace AI.HR.Repoistories;

/// <summary>
/// Data persistence operations for users.
/// </summary>
public interface IUserRepository
{
    /// <summary>Returns the user with the given Email, or null if not found.</summary>
    Task<UserItem?> GetByEmail(string email);

    /// <summary>Inserts a new user and returns the persisted UserItem.</summary>
    Task<UserItem> Create(UserItem userItem);

    /// <summary>Updates an existing user by Email and returns the persisted UserItem, or null if not found.</summary>
    Task<UserItem?> Update(UserItem userItem);

    /// <summary>Deletes a user by Email. Returns true if a user was deleted.</summary>
    Task<bool> Delete(string email);

    /// <summary>Creates the user if the Email doesn't exist yet, otherwise updates it.</summary>
    Task<UserItem> Upsert(UserItem userItem);

    /// <summary>Updates only the PasswordHash for the user with the given Email. Returns true if a user was found and updated.</summary>
    Task<bool> UpdatePassword(string email, string passwordHash);
}
