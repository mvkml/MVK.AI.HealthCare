using HC.AI.MAPI.Llm;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace HC.AI.MAPI.Prompt.Doctor;

public class DoctorPromptProvider : IDoctorPromptProvider
{
    private const string SystemPrompt =
        "You are a helpful healthcare AI assistant for doctors. Respond concisely and " +
        "professionally.";

    private readonly Kernel _kernel;

    public DoctorPromptProvider(IOptions<OllamaOptions> ollamaOptions)
    {
        var options = ollamaOptions.Value;
        _kernel = Kernel.CreateBuilder()
            .AddOllamaChatCompletion(modelId: options.Model, endpoint: new Uri(options.BaseUrl))
            .Build();
    }

    public string GetSystemPrompt() => SystemPrompt;

    public async Task<string> GetChatResponseByPrompt(string message)
    {
        var chatHistory = new ChatHistory(SystemPrompt);
        chatHistory.AddUserMessage(message);

        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
        return response.Content ?? string.Empty;
    }
}
