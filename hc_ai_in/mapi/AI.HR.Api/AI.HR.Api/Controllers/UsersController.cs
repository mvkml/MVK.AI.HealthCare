using AI.HR.BL;
using AI.HR.Models;
using AI.HR.Repoistories;
using Microsoft.AspNetCore.Mvc;

namespace AI.HR.Api.Controllers;

/// <summary>
/// Handles HTTP requests for user operations (Sign Up, get roles, etc.).
/// </summary>
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserValidationService _userValidationService;
    private readonly IUserBL _userBL;
    private readonly IRoleRepository _roleRepository;

    /// <summary>Creates the controller with the given validation service, business layer, and role repository.</summary>
    public UsersController(IUserValidationService userValidationService, IUserBL userBL, IRoleRepository roleRepository)
    {
        _userValidationService = userValidationService;
        _userBL = userBL;
        _roleRepository = roleRepository;
    }

    /// <summary>Signs up a new user.</summary>
    [HttpPost("signup")]
    public async Task<ActionResult<UserResponse>> SignUp([FromBody] UserRequest userRequest)
    {
        var usersModel = new UsersModel { UserRequest = userRequest };

        usersModel = _userValidationService.Validate(usersModel);
        if (usersModel.IsNotValid)
        {
            return BadRequest(new UserResponse
            {
                IsNotValid = true,
                Message = usersModel.Message,
            });
        }

        usersModel = await _userBL.SignUp(usersModel);
        return Ok(usersModel.UserResponse);
    }

    /// <summary>Logs in a user.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
    {
        var loginModel = new LoginModel { LoginRequest = loginRequest };

        loginModel = _userValidationService.ValidateLogin(loginModel);
        if (loginModel.IsNotValid)
        {
            return BadRequest(new LoginResponse
            {
                IsNotValid = true,
                Message = loginModel.Message,
            });
        }

        loginModel = await _userBL.Login(loginModel);
        if (loginModel.IsNotValid)
        {
            return Unauthorized(new LoginResponse
            {
                IsNotValid = true,
                Message = loginModel.Message,
            });
        }

        return Ok(loginModel.LoginResponse);
    }

    /// <summary>Confirms an account exists for the given email before the client proceeds to Reset Password.</summary>
    [HttpPost("forgot-password")]
    public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword([FromBody] ForgotPasswordRequest forgotPasswordRequest)
    {
        var forgotPasswordModel = new ForgotPasswordModel { ForgotPasswordRequest = forgotPasswordRequest };

        forgotPasswordModel = _userValidationService.ValidateForgotPassword(forgotPasswordModel);
        if (forgotPasswordModel.IsNotValid)
        {
            return BadRequest(new ForgotPasswordResponse
            {
                IsNotValid = true,
                Message = forgotPasswordModel.Message,
            });
        }

        forgotPasswordModel = await _userBL.ForgotPassword(forgotPasswordModel);
        if (forgotPasswordModel.IsNotValid)
        {
            return NotFound(new ForgotPasswordResponse
            {
                IsNotValid = true,
                Message = forgotPasswordModel.Message,
            });
        }

        return Ok(forgotPasswordModel.ForgotPasswordResponse);
    }

    /// <summary>Resets a user's password given their email and a new password.</summary>
    [HttpPost("reset-password")]
    public async Task<ActionResult<ResetPasswordResponse>> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
    {
        var resetPasswordModel = new ResetPasswordModel { ResetPasswordRequest = resetPasswordRequest };

        resetPasswordModel = _userValidationService.ValidateResetPassword(resetPasswordModel);
        if (resetPasswordModel.IsNotValid)
        {
            return BadRequest(new ResetPasswordResponse
            {
                IsNotValid = true,
                Message = resetPasswordModel.Message,
            });
        }

        resetPasswordModel = await _userBL.ResetPassword(resetPasswordModel);
        if (resetPasswordModel.IsNotValid)
        {
            return NotFound(new ResetPasswordResponse
            {
                IsNotValid = true,
                Message = resetPasswordModel.Message,
            });
        }

        return Ok(resetPasswordModel.ResetPasswordResponse);
    }

    /// <summary>Gets all roles (e.g. to populate the Sign Up role dropdown).</summary>
    [HttpGet("roles")]
    public async Task<ActionResult<RolesModel>> GetRoles()
    {
        var roleItems = await _roleRepository.GetAll();
        return Ok(new RolesModel { RoleItems = roleItems });
    }
}
