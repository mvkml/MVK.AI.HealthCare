using HC.AI.MAPI.Models;
using HC.AI.MAPI.Semantic.Factory;
using HC.AI.MAPI.SemanticProcess.Mapping;
using Microsoft.SemanticKernel;

namespace HC.AI.MAPI.SemanticProcess;

public class SemanticProcessService : ISemanticProcessService
{
    private readonly IKernalFactory _kernalFactory;

    public SemanticProcessService(IKernalFactory kernalFactory)
    {
        _kernalFactory = kernalFactory;
    }

    public async Task<string> ExecutePromptAsync(LLMOptions llmOptions, string promptTemplate, PromptItem? promptItem = null)
    {
        var kernel = _kernalFactory.CreateKernel(llmOptions);

        var executionSettings = promptItem is not null
            ? PromptItemMapper.ToExecutionSettings(promptItem)
            : null;

        var function = kernel.CreateFunctionFromPrompt(promptTemplate, executionSettings);
        var result = await kernel.InvokeAsync(function);
        return result.ToString();
    }
}
