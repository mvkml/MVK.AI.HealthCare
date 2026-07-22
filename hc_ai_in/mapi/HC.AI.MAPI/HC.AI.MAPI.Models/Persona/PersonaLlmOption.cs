namespace HC.AI.MAPI.Models.Persona;

/// <summary>
/// Mirrors the proposed <c>PersonaLLMOption</c> table (EPIC001 schema doc) — which LLM
/// model/provider a persona uses for a given <see cref="ModelRole"/> (Classification vs
/// Executor). <see cref="PersonaPromptTypeId"/> is null for Classification (single router
/// model) and set for Executor (one row per prompt-type/agent).
/// </summary>
public class PersonaLlmOption
{
    public int PersonaLlmOptionId { get; set; }
    public int RoleId { get; set; }
    public string ModelRole { get; set; } = string.Empty;
    public int? PersonaPromptTypeId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = true;
    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;
}
