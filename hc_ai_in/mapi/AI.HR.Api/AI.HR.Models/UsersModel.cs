namespace AI.HR.Models;

/// <summary>
/// Carrier model passed between Controller, Validation Service, and Business Layer.
/// Wraps the incoming request and/or the resulting single/multiple user data,
/// along with the common BaseModel status.
/// </summary>
public class UsersModel : BaseModel
{
    /// <summary>Incoming request data (e.g. from Sign Up), set by the Controller.</summary>
    public UserRequest UserRequest { get; set; } = new();

    /// <summary>Use when the response contains a single user.</summary>
    public UserItem UserItem { get; set; } = new();

    /// <summary>Use when the response contains multiple users.</summary>
    public List<UserItem> UserItems { get; set; } = new();

    /// <summary>Outgoing response data built by the Business Layer, returned to the Controller.</summary>
    public UserResponse UserResponse { get; set; } = new();
}
