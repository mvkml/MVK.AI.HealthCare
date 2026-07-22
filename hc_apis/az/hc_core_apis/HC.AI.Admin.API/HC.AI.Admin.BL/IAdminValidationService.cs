using HC.AI.Admin.Models;

namespace HC.AI.Admin.BL;

/// <summary>
/// Validates admin sign-up and login requests.
/// </summary>
public interface IAdminValidationService
{
    /// <summary>Validates AdminSignUpModel.AdminSignUpRequest, setting IsNotValid/Message on the model.</summary>
    Task<AdminSignUpModel> Validate(AdminSignUpModel adminSignUpModel);

    /// <summary>Validates AdminLoginModel.AdminLoginRequest, setting IsNotValid/Message on the model.</summary>
    AdminLoginModel ValidateLogin(AdminLoginModel adminLoginModel);
}
