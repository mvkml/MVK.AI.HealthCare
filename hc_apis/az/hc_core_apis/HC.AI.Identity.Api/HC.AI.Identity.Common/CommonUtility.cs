namespace HC.AI.Identity.Common;

/// <summary>
/// See <see cref="ICommonUtility"/>.
/// </summary>
public class CommonUtility : ICommonUtility
{
    public string GenerateDcn()
    {
        var now = DateTime.UtcNow;
        return $"DCN-{now:yy}-{now:MM}-{now:dd}-{now:HH}-{now:mm}-{now:ss}";
    }

    public string GenerateGrp()
    {
        var now = DateTime.UtcNow;
        return $"GRP-{now:yy}-{now:MM}-{now:dd}-{now:HH}-{now:mm}-{now:ss}";
    }
}
