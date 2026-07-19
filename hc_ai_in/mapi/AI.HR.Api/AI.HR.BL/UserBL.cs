using AI.HR.BL.Security;
using AI.HR.Models;
using AI.HR.Repoistories;

namespace AI.HR.BL;

/// <summary>
/// Business logic for user operations. Maps UserRequest to UserItem
/// and delegates persistence to IUserRepository.
/// </summary>
public class UserBL : IUserBL
{
    /// <summary>Default role assigned to self-service Sign Up ("Other", seeded RoleId 7).</summary>
    private const int DefaultSignUpRoleId = 7;

    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>Creates the business layer with the given IUserRepository, ITokenService, and IPasswordHasher.</summary>
    public UserBL(IUserRepository userRepository, ITokenService tokenService, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    /// <summary>Signs up a user: maps the request to a UserItem, upserts it, and builds the response.</summary>
    public async Task<UsersModel> SignUp(UsersModel usersModel)
    {
        usersModel.UserItem = ToUserItem(usersModel.UserRequest, _passwordHasher);

        var savedItem = await _userRepository.Upsert(usersModel.UserItem);
        usersModel.UserItem = savedItem;

        usersModel.UserResponse = ToUserResponse(savedItem);
        usersModel.IsNotValid = false;
        usersModel.Message = "User signed up successfully.";

        return usersModel;
    }

    /// <summary>Logs in a user: looks up by Email, verifies the password, and builds the response.</summary>
    public async Task<LoginModel> Login(LoginModel loginModel)
    {
        var userItem = await _userRepository.GetByEmail(loginModel.LoginRequest.Email);

        if (userItem is null || !_passwordHasher.Verify(loginModel.LoginRequest.Password, userItem.PasswordHash))
        {
            loginModel.IsNotValid = true;
            loginModel.Message = "Invalid email or password.";
            return loginModel;
        }

        if (!userItem.IsActive)
        {
            loginModel.IsNotValid = true;
            loginModel.Message = "This account is inactive.";
            return loginModel;
        }

        loginModel.LoginResponse = ToLoginResponse(userItem, _tokenService.GenerateToken(userItem));
        loginModel.IsNotValid = false;
        loginModel.Message = "Login successful.";

        return loginModel;
    }

    /// <summary>Confirms an account exists for the given Email before the client proceeds to Reset Password.</summary>
    public async Task<ForgotPasswordModel> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
    {
        var userItem = await _userRepository.GetByEmail(forgotPasswordModel.ForgotPasswordRequest.Email);

        if (userItem is null)
        {
            forgotPasswordModel.IsNotValid = true;
            forgotPasswordModel.Message = "No account found with this email.";
            return forgotPasswordModel;
        }

        forgotPasswordModel.ForgotPasswordResponse = new ForgotPasswordResponse
        {
            IsNotValid = false,
            Message = "Account found. You can now set a new password.",
        };
        forgotPasswordModel.IsNotValid = false;
        forgotPasswordModel.Message = forgotPasswordModel.ForgotPasswordResponse.Message;

        return forgotPasswordModel;
    }

    /// <summary>Resets a user's password: looks up by Email and overwrites the PasswordHash.</summary>
    public async Task<ResetPasswordModel> ResetPassword(ResetPasswordModel resetPasswordModel)
    {
        var request = resetPasswordModel.ResetPasswordRequest;
        var updated = await _userRepository.UpdatePassword(request.Email, _passwordHasher.Hash(request.NewPassword));

        if (!updated)
        {
            resetPasswordModel.IsNotValid = true;
            resetPasswordModel.Message = "No account found with this email.";
            return resetPasswordModel;
        }

        resetPasswordModel.ResetPasswordResponse = new ResetPasswordResponse
        {
            IsNotValid = false,
            Message = "Password reset successfully.",
        };
        resetPasswordModel.IsNotValid = false;
        resetPasswordModel.Message = resetPasswordModel.ResetPasswordResponse.Message;

        return resetPasswordModel;
    }

    private static LoginResponse ToLoginResponse(UserItem userItem, string token) => new()
    {
        UserId = userItem.UserId,
        FullName = userItem.FullName,
        Email = userItem.Email,
        Company = userItem.Company,
        RoleId = userItem.RoleId,
        Token = token,
    };

    private static UserItem ToUserItem(UserRequest request, IPasswordHasher passwordHasher) => new()
    {
        FullName = request.FullName,
        Email = request.Email,
        Company = request.Company,
        PasswordHash = passwordHasher.Hash(request.Password),
        RoleId = request.RoleId > 0 ? request.RoleId : DefaultSignUpRoleId,
    };

    private static UserResponse ToUserResponse(UserItem userItem) => new()
    {
        UserId = userItem.UserId,
        FullName = userItem.FullName,
        Email = userItem.Email,
        Company = userItem.Company,
        CreatedAt = userItem.CreatedAt,
        UpdatedAt = userItem.UpdatedAt,
        IsActive = userItem.IsActive,
    };
}
