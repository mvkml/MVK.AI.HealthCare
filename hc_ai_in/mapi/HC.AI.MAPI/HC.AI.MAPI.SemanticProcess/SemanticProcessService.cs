using HC.AI.MAPI.Llm;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace HC.AI.MAPI.SemanticProcess;

public class SemanticProcessService : ISemanticProcessService
{
    private readonly Kernel _kernel;

    public SemanticProcessService(IOptions<OllamaOptions> ollamaOptions)
    {
        var options = ollamaOptions.Value;
        _kernel = Kernel.CreateBuilder()
            .AddOllamaChatCompletion(modelId: options.Model, endpoint: new Uri(options.BaseUrl))
            .Build();
    }

    public async Task<string> ExecutePromptAsync(string promptTemplate, IDictionary<string, object?> arguments)
    {
        var function = _kernel.CreateFunctionFromPrompt(promptTemplate);

        var kernelArguments = new KernelArguments();
        foreach (var (key, value) in arguments)
        {
            kernelArguments[key] = value;
        }

        var result = await _kernel.InvokeAsync(function, kernelArguments);
        return result.ToString();
    }
}
