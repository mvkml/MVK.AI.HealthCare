namespace HC.AI.MAPI.Models;

public class PromptRequest
{
    public string Message { get; set; } = string.Empty;
    public string Persona { get; set; } = string.Empty;
    public int MaxTokens { get; set; } = 200;
    public double Temperature { get; set; } = 0.3;
    public double TopP { get; set; } = 0.9;
    public int TopK { get; set; } = 40;
    public double FrequencyPenalty { get; set; }
    public double PresencePenalty { get; set; }
    public List<string> StopSequences { get; set; } = new();
    public bool Stream { get; set; }
    public int? Seed { get; set; }
}
