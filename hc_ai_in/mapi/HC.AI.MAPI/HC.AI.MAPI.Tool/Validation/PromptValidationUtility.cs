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
            return result;
        }

        if (request.Temperature < 0 || request.Temperature > 2)
        {
            result.IsValid = false;
            result.Errors.Add("Temperature must be between 0 and 2.");
        }

        if (request.TopP < 0 || request.TopP > 1)
        {
            result.IsValid = false;
            result.Errors.Add("TopP must be between 0 and 1.");
        }

        if (request.TopK < 0)
        {
            result.IsValid = false;
            result.Errors.Add("TopK must be non-negative.");
        }

        if (request.MaxTokens < 0)
        {
            result.IsValid = false;
            result.Errors.Add("MaxTokens must be non-negative.");
        }

        return result;
    }
}
