using HC.AI.MAPI.Common;
using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.Services.Mapping;

/// <summary>
/// Owns the structural mapping <see cref="DoctorService"/> needs: request → item, and
/// assembling the <see cref="PromptModel"/> shell. Pulled out on its own so the Service stays
/// pure orchestration (single responsibility). Injected via DI like every other class in this
/// codebase, rather than a static utility.
///
/// This is the one and only place <see cref="PromptModel.PromptRequest"/> gets read — every
/// field it carries (Message, generation parameters) gets copied onto
/// <see cref="PromptModel.PromptItem"/> here. From <c>DoctorService</c> onward, nothing reads
/// <c>PromptRequest</c> again; the item is self-sufficient.
///
/// Deciding *which* LLM/model/provider to use (<see cref="PromptItem.LLMOptions"/> and
/// <see cref="PromptItem.LLMProvider"/>) is a business decision, not a mapping — that lives in
/// <c>HC.AI.MAPI.BL.LLMModel.ILLMModelBL</c>. This mapper does not depend on <c>OllamaOptions</c>
/// (or any provider config) at all, precisely so it can't duplicate that decision.
///
/// Per ADR001 (Context Object pattern), every method here takes and returns the full
/// <see cref="PromptModel"/> envelope, except <see cref="ToPromptModel"/> — the one
/// construction-point exception, since no model exists yet at that call.
/// </summary>
public class DoctorPromptMapper : IDoctorPromptMapper
{
    public PromptModel ToPromptItem(PromptModel model)
    {
        var request = model.PromptRequest;

        var modelKey = string.Equals(request.Persona, APIConstants.PatientPersonaName, StringComparison.OrdinalIgnoreCase)
            ? APIConstants.PatientExecutorPersonaName
            : APIConstants.DoctorExecutorPersonaName;

        model.PromptItem = new PromptItem
        {
            PromptKey = APIConstants.DoctorChatPromptKey,
            Persona = request.Persona,
            ModelKey = modelKey,
            Source = APIConstants.DoctorServiceSourceName,
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
