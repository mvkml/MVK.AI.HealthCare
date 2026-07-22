using HC.AI.Admin.Models;

namespace HC.AI.Admin.BL;

/// <summary>
/// Business logic for admin operations.
/// </summary>
public interface IAdminBL
{
    /// <summary>Signs up an admin: maps the request to an AdminItem, persists it, and builds the response.</summary>
    Task<AdminSignUpModel> SignUp(AdminSignUpModel adminSignUpModel);

    /// <summary>Logs in an admin: looks up by Email, verifies the password, and builds the response.</summary>
    Task<AdminLoginModel> Login(AdminLoginModel adminLoginModel);
}
