using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.Tool.Validation;

public interface IPromptValidationUtility
{
    ValidationResult Validate(PromptRequest request);
}
