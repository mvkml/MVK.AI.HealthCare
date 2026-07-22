namespace HC.AI.MAPI.Models.Persona;

/// <summary>
/// Mirrors the proposed <c>PersonaPrompt</c> table (EPIC001 schema doc) — the prompt content
/// for a persona + <see cref="ModelRole"/> (+ prompt-type, for Executor). Named
/// <c>PersonaPromptRecord</c> rather than <c>PersonaPrompt</c> to avoid confusion with the
/// unrelated <c>HC.AI.MAPI.Prompt</c> project (Doctor's hardcoded system prompt provider).
/// </summary>
public class PersonaPromptRecord
{
    public int PersonaPromptId { get; set; }
    public int RoleId { get; set; }
    public string ModelRole { get; set; } = string.Empty;
    public int? PersonaPromptTypeId { get; set; }
    public string PromptText { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
}
