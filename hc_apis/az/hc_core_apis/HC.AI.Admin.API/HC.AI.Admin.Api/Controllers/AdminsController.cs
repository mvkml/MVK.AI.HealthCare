using HC.AI.Admin.BL;
using HC.AI.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace HC.AI.Admin.Api.Controllers;

/// <summary>
/// Handles HTTP requests for admin operations (Sign Up, Login).
/// </summary>
[ApiController]
[Route("api/admins")]
public class AdminsController : ControllerBase
{
    private readonly IAdminValidationService _adminValidationService;
    private readonly IAdminBL _adminBL;

    /// <summary>Creates the controller with the given validation service and business layer.</summary>
    public AdminsController(IAdminValidationService adminValidationService, IAdminBL adminBL)
    {
        _adminValidationService = adminValidationService;
        _adminBL = adminBL;
    }

    /// <summary>Signs up a new admin.</summary>
    [HttpPost("signup")]
    public async Task<ActionResult<AdminSignUpResponse>> SignUp([FromBody] AdminSignUpRequest adminSignUpRequest)
    {
        var adminSignUpModel = new AdminSignUpModel { AdminSignUpRequest = adminSignUpRequest };

        adminSignUpModel = await _adminValidationService.Validate(adminSignUpModel);
        if (adminSignUpModel.IsNotValid)
        {
            return BadRequest(new AdminSignUpResponse
            {
                IsNotValid = true,
                Message = adminSignUpModel.Message,
            });
        }

        adminSignUpModel = await _adminBL.SignUp(adminSignUpModel);
        return Ok(adminSignUpModel.AdminSignUpResponse);
    }

    /// <summary>Logs in an admin.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<AdminLoginResponse>> Login([FromBody] AdminLoginRequest adminLoginRequest)
    {
        var adminLoginModel = new AdminLoginModel { AdminLoginRequest = adminLoginRequest };

        adminLoginModel = _adminValidationService.ValidateLogin(adminLoginModel);
        if (adminLoginModel.IsNotValid)
        {
            return BadRequest(new AdminLoginResponse
            {
                IsNotValid = true,
                Message = adminLoginModel.Message,
            });
        }

        adminLoginModel = await _adminBL.Login(adminLoginModel);
        if (adminLoginModel.IsNotValid)
        {
            return Unauthorized(new AdminLoginResponse
            {
                IsNotValid = true,
                Message = adminLoginModel.Message,
            });
        }

        return Ok(adminLoginModel.AdminLoginResponse);
    }
}
