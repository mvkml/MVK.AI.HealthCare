using System.Text.RegularExpressions;
using HC.AI.Admin.Models;
using HC.AI.Admin.Repositories;

namespace HC.AI.Admin.BL;

/// <summary>
/// Validates admin sign-up and login requests, setting IsNotValid/Message accordingly.
/// </summary>
public class AdminValidationService : IAdminValidationService
{
    private static readonly Regex EmailPattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private readonly IAdminRepository _adminRepository;

    /// <summary>Creates the service with the given IAdminRepository (used to check for duplicate emails).</summary>
    public AdminValidationService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    /// <summary>Validates AdminSignUpModel.AdminSignUpRequest, setting IsNotValid/Message on the model.</summary>
    public async Task<AdminSignUpModel> Validate(AdminSignUpModel adminSignUpModel)
    {
        var request = adminSignUpModel.AdminSignUpRequest;

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            adminSignUpModel.IsNotValid = true;
            adminSignUpModel.Message = "FullName is required.";
            return adminSignUpModel;
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !EmailPattern.IsMatch(request.Email))
        {
            adminSignUpModel.IsNotValid = true;
            adminSignUpModel.Message = "A valid Email is required.";
            return adminSignUpModel;
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            adminSignUpModel.IsNotValid = true;
            adminSignUpModel.Message = "Password is required and must be at least 8 characters.";
            return adminSignUpModel;
        }

        if (await _adminRepository.ExistsByEmail(request.Email))
        {
            adminSignUpModel.IsNotValid = true;
            adminSignUpModel.Message = "An admin with this email already exists.";
            return adminSignUpModel;
        }

        adminSignUpModel.IsNotValid = false;
        adminSignUpModel.Message = "Validation passed.";
        return adminSignUpModel;
    }

    /// <summary>Validates AdminLoginModel.AdminLoginRequest, setting IsNotValid/Message on the model.</summary>
    public AdminLoginModel ValidateLogin(AdminLoginModel adminLoginModel)
    {
        var request = adminLoginModel.AdminLoginRequest;

        if (string.IsNullOrWhiteSpace(request.Email) || !EmailPattern.IsMatch(request.Email))
        {
            adminLoginModel.IsNotValid = true;
            adminLoginModel.Message = "A valid Email is required.";
            return adminLoginModel;
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            adminLoginModel.IsNotValid = true;
            adminLoginModel.Message = "Password is required.";
            return adminLoginModel;
        }

        adminLoginModel.IsNotValid = false;
        adminLoginModel.Message = "Validation passed.";
        return adminLoginModel;
    }
}
