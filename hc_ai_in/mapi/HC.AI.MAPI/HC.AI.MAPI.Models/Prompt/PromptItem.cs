namespace HC.AI.MAPI.Models;

/// <summary>
/// The self-contained unit of work — everything a layer needs to execute, decoupled from
/// <see cref="PromptModel"/>. The model is the pipeline envelope for this one request's
/// sequential flow; the item is what would get handed to a parallel/fan-out worker on its own,
/// with no dependency on the rest of the model. From <c>DoctorService</c> onward, layers read
/// execution details (<see cref="Message"/>, the generation parameters, <see cref="LLMOptions"/>)
/// from here, never from <see cref="PromptModel.PromptRequest"/>.
/// </summary>
public class PromptItem
{
    public string PromptKey { get; set; } = string.Empty;
    public string Persona { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string LLMProvider { get; set; } = string.Empty;
    public Dictionary<string, string> TemplateVariables { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new();
    public string CorrelationId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public string Message { get; set; } = string.Empty;
    public int MaxTokens { get; set; }
    public double Temperature { get; set; }
    public double TopP { get; set; }
    public int TopK { get; set; }
    public double FrequencyPenalty { get; set; }
    public double PresencePenalty { get; set; }
    public List<string> StopSequences { get; set; } = new();
    public bool Stream { get; set; }
    public int? Seed { get; set; }
    public LLMOptions LLMOptions { get; set; } = new LLMOptions();
}
