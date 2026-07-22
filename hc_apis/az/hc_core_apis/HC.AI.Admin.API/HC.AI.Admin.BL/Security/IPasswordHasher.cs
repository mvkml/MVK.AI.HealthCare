namespace HC.AI.Admin.BL.Security;

/// <summary>
/// Hashes and verifies passwords.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Hashes the given plain-text password.</summary>
    string Hash(string password);

    /// <summary>Verifies the given plain-text password against a previously hashed value.</summary>
    bool Verify(string password, string hashedValue);
}
