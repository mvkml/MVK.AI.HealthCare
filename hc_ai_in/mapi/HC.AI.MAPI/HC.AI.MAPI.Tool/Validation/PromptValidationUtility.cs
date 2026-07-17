using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.Tool.Validation;

public class PromptValidationUtility : IPromptValidationUtility
{
    public ValidationResult Validate(PromptRequest request)
    {
        var result = new ValidationResult();

        if (request == null || string.IsNullOrWhiteSpace(request.Message))
        {
            result.IsValid = false;
            result.Errors.Add("Prompt text (Message) is required.");
        }

        return result;
    }
}
