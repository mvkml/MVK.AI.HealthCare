using HC.AI.MAPI.AL;
using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorAgent _doctorAgent;

    public DoctorService(IDoctorAgent doctorAgent)
    {
        _doctorAgent = doctorAgent;
    }

    public Task<string> HandleRequestAsync(string message)
    {
        return _doctorAgent.HandleRequestAsync(message);
    }

    public Task<string> BasicHandleRequestAsync(string message)
    {
        return _doctorAgent.BasicHandleRequestAsync(message);
    }

    public Task<string> GetChatResponseByPrompt(string message)
    {
        //return _doctorAgent.GetChatResponseByPrompt(message);
        throw new NotImplementedException();
    }

    public Task<PromptResponse> ProvidePromptAsync(PromptRequest request)
    {
        throw new NotImplementedException();
    }
}
