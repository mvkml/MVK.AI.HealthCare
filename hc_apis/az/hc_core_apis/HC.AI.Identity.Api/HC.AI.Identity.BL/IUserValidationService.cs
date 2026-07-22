using HC.AI.Identity.Models;

namespace HC.AI.Identity.BL;

/// <summary>
/// Validates the UserRequest carried on a UsersModel.
/// </summary>
public interface IUserValidationService
{
    /// <summary>Validates UsersModel.UserRequest, setting IsNotValid/Message on the model.</summary>
    UsersModel Validate(UsersModel usersModel);

    /// <summary>Validates LoginModel.LoginRequest, setting IsNotValid/Message on the model.</summary>
    LoginModel ValidateLogin(LoginModel loginModel);

    /// <summary>Validates ForgotPasswordModel.ForgotPasswordRequest, setting IsNotValid/Message on the model.</summary>
    ForgotPasswordModel ValidateForgotPassword(ForgotPasswordModel forgotPasswordModel);

    /// <summary>Validates ResetPasswordModel.ResetPasswordRequest, setting IsNotValid/Message on the model.</summary>
    ResetPasswordModel ValidateResetPassword(ResetPasswordModel resetPasswordModel);
}
