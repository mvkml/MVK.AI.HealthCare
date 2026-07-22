using HC.AI.MAPI.AL;
using HC.AI.MAPI.BL.LLMModel;
using HC.AI.MAPI.Models;
using HC.AI.MAPI.Services.Mapping;

namespace HC.AI.MAPI.Services;

/// <summary>
/// Patient-side counterpart to <see cref="DoctorService"/> — same
/// Mapper -> Model-Selection -> SemanticProcess pipeline, only
/// <see cref="ProvidePromptAsync"/> (the locked/structured-request shape; see
/// <see cref="IPatientService"/> for why the older demo-method shapes aren't mirrored).
/// </summary>
public class PatientService : IPatientService
{
    private readonly IPatientSemanticProcess _patientSemanticProcess;
    private readonly IPatientPromptMapper _patientPromptMapper;
    private readonly ILLMModelBL _llmModelBL;

    public PatientService(
        IPatientSemanticProcess patientSemanticProcess,
        IPatientPromptMapper patientPromptMapper,
        ILLMModelBL llmModelBL)
    {
        _patientSemanticProcess = patientSemanticProcess;
        _patientPromptMapper = patientPromptMapper;
        _llmModelBL = llmModelBL;
    }

    public async Task<PromptResponse> ProvidePromptAsync(PromptModel model)
    {
        model = _patientPromptMapper.ToPromptItem(model);
        model = _llmModelBL.GetModelDetails(model);
        model = await _patientSemanticProcess.ProcessAsync(model);
        return model.PromptResponse;
    }
}
