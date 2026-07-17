using HC.AI.MAPI.Llm;
using HC.AI.MAPI.Prompt.Doctor;

namespace HC.AI.MAPI.AL;

public class DoctorAgent : IDoctorAgent
{
    private readonly IOllamaClient _ollamaClient;
    private readonly IDoctorPromptProvider _doctorPromptProvider;

    public DoctorAgent(IOllamaClient ollamaClient, IDoctorPromptProvider doctorPromptProvider)
    {
        _ollamaClient = ollamaClient;
        _doctorPromptProvider = doctorPromptProvider;
    }

    public Task<string> BasicHandleRequestAsync(string message)
    {
        return Task.FromResult("Hello Doctor, this is your healthcare AI assistant. How can I help you today?");
    }

    public Task<string> HandleRequestAsync(string message)
    {
        return _ollamaClient.ChatAsync(_doctorPromptProvider.GetSystemPrompt(), message);
    }
}
