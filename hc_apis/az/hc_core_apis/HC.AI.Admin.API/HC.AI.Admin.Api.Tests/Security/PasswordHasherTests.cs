using HC.AI.Admin.BL.Security;

namespace HC.AI.Admin.Api.Tests.Security;

public class PasswordHasherTests
{
    private readonly IPasswordHasher _passwordHasher = new PasswordHasher();

    [Fact]
    public void Hash_SamePasswordTwice_ProducesDifferentHashes()
    {
        const string password = "SamePassword123";

        var firstHash = _passwordHasher.Hash(password);
        var secondHash = _passwordHasher.Hash(password);

        Assert.NotEqual(firstHash, secondHash);
    }

    [Fact]
    public void Verify_CorrectPassword_ReturnsTrue()
    {
        const string password = "CorrectPassword123";
        var hash = _passwordHasher.Hash(password);

        Assert.True(_passwordHasher.Verify(password, hash));
    }

    [Fact]
    public void Verify_WrongPassword_ReturnsFalse()
    {
        var hash = _passwordHasher.Hash("CorrectPassword123");

        Assert.False(_passwordHasher.Verify("WrongPassword123", hash));
    }
}
