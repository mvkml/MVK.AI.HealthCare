using HC.AI.MAPI.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace HC.AI.MAPI.SemanticProcess.Mapping;

/// <summary>
/// Maps between <see cref="PromptItem"/>/<see cref="PromptResponse"/> (our internal execution
/// unit, per ADR001 — never <see cref="PromptRequest"/>, which stops at the Service layer) and
/// Semantic Kernel's execution-time types. Kept as a small static mapper rather than AutoMapper
/// per the project's Implementation Guide (hc_agile/architecture/tech_stack).
/// Only Ollama is wired today; if a second provider is added, this becomes provider-specific
/// (see the Strategy pattern entry in the guide) rather than growing an if/else here.
/// </summary>
public static class PromptItemMapper
{
    /// <summary>
    /// Ollama has no direct equivalent for FrequencyPenalty, PresencePenalty, or Seed, so those
    /// PromptItem fields are intentionally not mapped — documented gap, not an oversight.
    /// </summary>
    public static OllamaPromptExecutionSettings ToExecutionSettings(PromptItem promptItem)
    {
        var settings = new OllamaPromptExecutionSettings();

        if (promptItem.Temperature > 0)
        {
            settings.Temperature = (float)promptItem.Temperature;
        }

        if (promptItem.TopP > 0)
        {
            settings.TopP = (float)promptItem.TopP;
        }

        if (promptItem.TopK > 0)
        {
            settings.TopK = promptItem.TopK;
        }

        if (promptItem.MaxTokens > 0)
        {
            settings.NumPredict = promptItem.MaxTokens;
        }

        if (promptItem.StopSequences.Count > 0)
        {
            settings.Stop = promptItem.StopSequences;
        }

        return settings;
    }

    public static PromptResponse ToPromptResponse(string content, string modelUsed, long latencyMs)
    {
        return new PromptResponse
        {
            Content = content,
            IsSuccess = true,
            ModelUsed = modelUsed,
            LatencyMs = latencyMs,
        };
    }

    public static PromptResponse ToErrorPromptResponse(string errorMessage, long latencyMs)
    {
        return new PromptResponse
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            LatencyMs = latencyMs,
        };
    }
}
