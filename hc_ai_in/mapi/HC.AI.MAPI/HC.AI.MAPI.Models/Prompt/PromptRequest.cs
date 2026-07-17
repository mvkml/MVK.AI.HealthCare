namespace HC.AI.MAPI.Models;

public class PromptRequest
{
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
}
