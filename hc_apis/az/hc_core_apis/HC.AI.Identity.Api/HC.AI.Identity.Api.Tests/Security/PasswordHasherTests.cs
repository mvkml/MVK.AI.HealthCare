using HC.AI.Identity.BL.Security;

namespace HC.AI.Identity.Api.Tests.Security;

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
}
