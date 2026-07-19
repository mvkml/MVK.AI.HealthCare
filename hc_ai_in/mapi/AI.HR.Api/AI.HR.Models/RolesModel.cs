namespace AI.HR.Models;

/// <summary>
/// API response model wrapping the list of roles, along with the common BaseModel status.
/// </summary>
public class RolesModel : BaseModel
{
    /// <summary>List of available roles.</summary>
    public List<RoleItem> RoleItems { get; set; } = new();
}
