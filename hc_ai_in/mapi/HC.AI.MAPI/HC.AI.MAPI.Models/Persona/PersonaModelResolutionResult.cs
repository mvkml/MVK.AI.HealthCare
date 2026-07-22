namespace HC.AI.MAPI.Models.Persona;

/// <summary>
/// What resolving a persona + <see cref="ModelRole"/> (+ prompt-type, for Executor) produces:
/// the model/provider to call and the prompt to call it with. <see cref="IsResolved"/> is false
/// when no active config exists for the given combination — callers decide what to do about
/// that (US012/US013 both flag "fallback behavior needs Product Owner sign-off, not assumed
/// here"; this mock implementation does not invent that decision).
/// </summary>
public class PersonaModelResolutionResult
{
    public bool IsResolved { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string PromptText { get; set; } = string.Empty;
}
