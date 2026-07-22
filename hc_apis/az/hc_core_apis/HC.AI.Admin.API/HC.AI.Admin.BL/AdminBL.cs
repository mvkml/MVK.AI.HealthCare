using HC.AI.Admin.BL.Security;
using HC.AI.Admin.Models;
using HC.AI.Admin.Repositories;

namespace HC.AI.Admin.BL;

/// <summary>
/// Business logic for admin operations. Maps AdminSignUpRequest to AdminItem
/// and delegates persistence to IAdminRepository.
/// </summary>
public class AdminBL : IAdminBL
{
    private readonly IAdminRepository _adminRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>Creates the business layer with the given IAdminRepository, ITokenService, and IPasswordHasher.</summary>
    public AdminBL(IAdminRepository adminRepository, ITokenService tokenService, IPasswordHasher passwordHasher)
    {
        _adminRepository = adminRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    /// <summary>Signs up an admin: maps the request to an AdminItem, persists it, and builds the response.</summary>
    public async Task<AdminSignUpModel> SignUp(AdminSignUpModel adminSignUpModel)
    {
        var request = adminSignUpModel.AdminSignUpRequest;

        var adminItem = new AdminItem
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
        };

        var savedItem = await _adminRepository.Create(adminItem);
        adminSignUpModel.AdminItem = savedItem;

        adminSignUpModel.AdminSignUpResponse = new AdminSignUpResponse
        {
            AdminId = savedItem.AdminId,
            FullName = savedItem.FullName,
            Email = savedItem.Email,
            CreatedAt = savedItem.CreatedAt,
            IsActive = savedItem.IsActive,
        };
        adminSignUpModel.IsNotValid = false;
        adminSignUpModel.Message = "Admin signed up successfully.";

        return adminSignUpModel;
    }

    /// <summary>Logs in an admin: looks up by Email, verifies the password, and builds the response.</summary>
    public async Task<AdminLoginModel> Login(AdminLoginModel adminLoginModel)
    {
        var adminItem = await _adminRepository.GetByEmail(adminLoginModel.AdminLoginRequest.Email);

        if (adminItem is null || !_passwordHasher.Verify(adminLoginModel.AdminLoginRequest.Password, adminItem.PasswordHash))
        {
            adminLoginModel.IsNotValid = true;
            adminLoginModel.Message = "Invalid email or password.";
            return adminLoginModel;
        }

        if (!adminItem.IsActive)
        {
            adminLoginModel.IsNotValid = true;
            adminLoginModel.Message = "This account is inactive.";
            return adminLoginModel;
        }

        adminLoginModel.AdminLoginResponse = new AdminLoginResponse
        {
            AdminId = adminItem.AdminId,
            FullName = adminItem.FullName,
            Email = adminItem.Email,
            Token = _tokenService.GenerateToken(adminItem),
        };
        adminLoginModel.IsNotValid = false;
        adminLoginModel.Message = "Login successful.";

        return adminLoginModel;
    }
}
