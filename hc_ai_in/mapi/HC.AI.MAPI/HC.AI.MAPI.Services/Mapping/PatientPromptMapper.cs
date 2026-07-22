using HC.AI.MAPI.Common;
using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.Services.Mapping;

/// <summary>
/// Patient-side counterpart to <see cref="DoctorPromptMapper"/>. Unlike
/// <c>DoctorPromptMapper</c> (which reads <c>request.Persona</c> to decide the
/// <see cref="PromptItem.ModelKey"/>, because the Doctor and Patient endpoints currently share
/// one controller's mapper in that flow), this mapper hardcodes
/// <see cref="APIConstants.PatientPersonaName"/> / <see cref="APIConstants.PatientExecutorPersonaName"/>
/// outright — it exists specifically for <c>PatientController</c>, so the request body's
/// <c>Persona</c> field is not trusted to decide it. Same reasoning as the fix applied to
/// <c>DoctorPromptMapper</c>'s earlier bug: an endpoint should decide its own persona from which
/// endpoint was hit, not from client-supplied data.
/// </summary>
public class PatientPromptMapper : IPatientPromptMapper
{
    public PromptModel ToPromptItem(PromptModel model)
    {
        var request = model.PromptRequest;

        model.PromptItem = new PromptItem
        {
            PromptKey = APIConstants.PatientChatPromptKey,
            Persona = APIConstants.PatientPersonaName,
            ModelKey = APIConstants.PatientExecutorPersonaName,
            Source = APIConstants.PatientServiceSourceName,
            CorrelationId = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,

            Message = request.Message,
            MaxTokens = request.MaxTokens,
            Temperature = request.Temperature,
            TopP = request.TopP,
            TopK = request.TopK,
            FrequencyPenalty = request.FrequencyPenalty,
            PresencePenalty = request.PresencePenalty,
            StopSequences = request.StopSequences,
            Stream = request.Stream,
            Seed = request.Seed,
        };

        return model;
    }

    public PromptModel ToPromptModel(PromptRequest request)
    {
        var model = new PromptModel { PromptRequest = request };
        return ToPromptItem(model);
    }
}
