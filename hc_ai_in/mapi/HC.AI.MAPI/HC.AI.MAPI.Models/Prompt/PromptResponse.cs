namespace HC.AI.MAPI.Models;

public class PromptResponse
{
    public string Content { get; set; } = string.Empty;
    public string FinishReason { get; set; } = string.Empty;
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
    public string ModelUsed { get; set; } = string.Empty;
    public long LatencyMs { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
