using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.SemanticProcess;

public interface ISemanticProcessService
{
    /// <summary>
    /// Builds a Kernel for the given provider/model via <c>IKernalFactory</c>, then executes the
    /// prompt through it. <paramref name="promptItem"/> supplies generation parameters
    /// (Temperature, TopP, etc.) — never <c>PromptRequest</c>, which per ADR001 stops at the
    /// Service layer. Pass <c>null</c> to use provider defaults.
    /// </summary>
    Task<string> ExecutePromptAsync(LLMOptions llmOptions, string promptTemplate, PromptItem? promptItem = null);
}
