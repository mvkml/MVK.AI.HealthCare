namespace HC.AI.MAPI.Models;

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
}
