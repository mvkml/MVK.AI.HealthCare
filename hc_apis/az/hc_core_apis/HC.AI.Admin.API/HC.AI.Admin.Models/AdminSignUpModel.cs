namespace HC.AI.Admin.Models;

/// <summary>
/// Carrier model passed between Controller, Validation Service, and Business Layer for Sign Up.
/// </summary>
public class AdminSignUpModel : BaseModel
{
    /// <summary>Incoming request data, set by the Controller.</summary>
    public AdminSignUpRequest AdminSignUpRequest { get; set; } = new();

    /// <summary>The persisted admin, set by the Business Layer.</summary>
    public AdminItem AdminItem { get; set; } = new();

    /// <summary>Outgoing response data built by the Business Layer, returned to the Controller.</summary>
    public AdminSignUpResponse AdminSignUpResponse { get; set; } = new();
}
