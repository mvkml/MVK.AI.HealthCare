using System.Text.RegularExpressions;
using HC.AI.Identity.Models;

namespace HC.AI.Identity.BL;

/// <summary>
/// Validates the UserRequest carried on a UsersModel and sets the
/// IsNotValid/Message status accordingly.
/// </summary>
public class UserValidationService : IUserValidationService
{
    private static readonly Regex EmailPattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    /// <summary>Validates UsersModel.UserRequest, setting IsNotValid/Message on the model.</summary>
    public UsersModel Validate(UsersModel usersModel)
    {
        var request = usersModel.UserRequest;

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            usersModel.IsNotValid = true;
            usersModel.Message = "FullName is required.";
            return usersModel;
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !EmailPattern.IsMatch(request.Email))
        {
            usersModel.IsNotValid = true;
            usersModel.Message = "A valid Email is required.";
            return usersModel;
        }

        if (string.IsNullOrWhiteSpace(request.Company))
        {
            usersModel.IsNotValid = true;
            usersModel.Message = "Company is required.";
            return usersModel;
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            usersModel.IsNotValid = true;
            usersModel.Message = "Password is required and must be at least 8 characters.";
            return usersModel;
        }

        usersModel.IsNotValid = false;
        usersModel.Message = "Validation passed.";
        return usersModel;
    }

    /// <summary>Validates LoginModel.LoginRequest, setting IsNotValid/Message on the model.</summary>
    public LoginModel ValidateLogin(LoginModel loginModel)
    {
        var request = loginModel.LoginRequest;

        if (string.IsNullOrWhiteSpace(request.Email) || !EmailPattern.IsMatch(request.Email))
        {
            loginModel.IsNotValid = true;
            loginModel.Message = "A valid Email is required.";
            return loginModel;
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            loginModel.IsNotValid = true;
            loginModel.Message = "Password is required.";
            return loginModel;
        }

        loginModel.IsNotValid = false;
        loginModel.Message = "Validation passed.";
        return loginModel;
    }

    /// <summary>Validates ForgotPasswordModel.ForgotPasswordRequest, setting IsNotValid/Message on the model.</summary>
    public ForgotPasswordModel ValidateForgotPassword(ForgotPasswordModel forgotPasswordModel)
    {
        var request = forgotPasswordModel.ForgotPasswordRequest;

        if (string.IsNullOrWhiteSpace(request.Email) || !EmailPattern.IsMatch(request.Email))
        {
            forgotPasswordModel.IsNotValid = true;
            forgotPasswordModel.Message = "A valid Email is required.";
            return forgotPasswordModel;
        }

        forgotPasswordModel.IsNotValid = false;
        forgotPasswordModel.Message = "Validation passed.";
        return forgotPasswordModel;
    }

    /// <summary>Validates ResetPasswordModel.ResetPasswordRequest, setting IsNotValid/Message on the model.</summary>
    public ResetPasswordModel ValidateResetPassword(ResetPasswordModel resetPasswordModel)
    {
        var request = resetPasswordModel.ResetPasswordRequest;

        if (string.IsNullOrWhiteSpace(request.Email) || !EmailPattern.IsMatch(request.Email))
        {
            resetPasswordModel.IsNotValid = true;
            resetPasswordModel.Message = "A valid Email is required.";
            return resetPasswordModel;
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
        {
            resetPasswordModel.IsNotValid = true;
            resetPasswordModel.Message = "New password is required and must be at least 8 characters.";
            return resetPasswordModel;
        }

        resetPasswordModel.IsNotValid = false;
        resetPasswordModel.Message = "Validation passed.";
        return resetPasswordModel;
    }
}
