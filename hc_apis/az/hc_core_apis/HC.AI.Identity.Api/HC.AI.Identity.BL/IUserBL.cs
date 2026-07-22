using HC.AI.Identity.Models;

namespace HC.AI.Identity.BL;

/// <summary>
/// Business logic for user operations.
/// </summary>
public interface IUserBL
{
    /// <summary>Signs up a user: maps the request to a UserItem, upserts it, and builds the response.</summary>
    Task<UsersModel> SignUp(UsersModel usersModel);

    /// <summary>Logs in a user: looks up by Email, verifies the password, and builds the response.</summary>
    Task<LoginModel> Login(LoginModel loginModel);

    /// <summary>Confirms an account exists for the given Email before the client proceeds to Reset Password.</summary>
    Task<ForgotPasswordModel> ForgotPassword(ForgotPasswordModel forgotPasswordModel);

    /// <summary>Resets a user's password: looks up by Email and overwrites the PasswordHash.</summary>
    Task<ResetPasswordModel> ResetPassword(ResetPasswordModel resetPasswordModel);
}
