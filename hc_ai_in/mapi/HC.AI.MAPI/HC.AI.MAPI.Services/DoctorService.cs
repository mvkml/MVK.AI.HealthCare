using HC.AI.MAPI.AL;
using HC.AI.MAPI.BL.LLMModel;
using HC.AI.MAPI.Models;
using HC.AI.MAPI.Services.Mapping;

namespace HC.AI.MAPI.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorAgent _doctorAgent;
    private readonly IDoctorSemanticProcess _doctorSemanticProcess;
    private readonly IDoctorPromptMapper _doctorPromptMapper;
    private readonly ILLMModelBL _llmModelBL;

    public DoctorService(
        IDoctorAgent doctorAgent,
        IDoctorSemanticProcess doctorSemanticProcess,
        IDoctorPromptMapper doctorPromptMapper,
        ILLMModelBL llmModelBL)
    {
        _doctorAgent = doctorAgent;
        _doctorSemanticProcess = doctorSemanticProcess;
        _doctorPromptMapper = doctorPromptMapper;
        _llmModelBL = llmModelBL;
    }

    public Task<string> HandleRequestAsync(string message)
    {
        return _doctorAgent.HandleRequestAsync(message);
    }

    public Task<string> BasicHandleRequestAsync(string message)
    {
        return _doctorAgent.BasicHandleRequestAsync(message);
    }

    public async Task<string> GetChatResponseByPrompt(string message)
    {
        var model = _doctorPromptMapper.ToPromptModel(new PromptRequest { Message = message });
        model = _llmModelBL.GetModelDetails(model);
        model = await _doctorSemanticProcess.ProcessAsync(model);
        return model.PromptResponse.IsSuccess ? model.PromptResponse.Content : $"Error: {model.PromptResponse.ErrorMessage}";
    }

    public async Task<PromptResponse> ProvidePromptAsync(PromptModel model)
    {
        model = _doctorPromptMapper.ToPromptItem(model);
        model = _llmModelBL.GetModelDetails(model);
        model = await _doctorSemanticProcess.ProcessAsync(model);
        return model.PromptResponse;
    }
}
