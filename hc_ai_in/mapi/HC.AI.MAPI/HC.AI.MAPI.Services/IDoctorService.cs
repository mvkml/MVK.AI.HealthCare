using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.Services;

public interface IDoctorService
{
    Task<string> HandleRequestAsync(string message);

    Task<string> BasicHandleRequestAsync(string message);
    Task<string> GetChatResponseByPrompt(string message);
    Task<PromptResponse> ProvidePromptAsync(PromptRequest request);
}
