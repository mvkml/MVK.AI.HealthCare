using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.AL;

public interface IDoctorSemanticProcess
{
    /// <summary>
    /// Executes the Doctor persona's chat flow using <see cref="PromptModel.PromptRequest"/>,
    /// <see cref="PromptModel.PromptItem"/>, and <see cref="PromptModel.LLMOptions"/>, and
    /// returns the same model with <see cref="PromptModel.PromptResponse"/> populated.
    /// </summary>
    Task<PromptModel> ProcessAsync(PromptModel model);
}
