namespace HC.AI.Identity.BL.Security;

/// <summary>
/// Hashes and verifies passwords.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Hashes a plain-text password.</summary>
    string Hash(string password);

    /// <summary>Verifies a plain-text password against a previously hashed value.</summary>
    bool Verify(string password, string hashedValue);
}
