using HC.AI.MAPI.Models.Persona;

namespace HC.AI.MAPI.BL.Persona;

/// <summary>
/// Implements the resolution steps from US012 (Classification) and US013 (Executor) against
/// <see cref="IPersonaLlmConfigProvider"/>. Resolution only — running the resolved model against
/// Ollama is the caller's job (this class doesn't know about Semantic Kernel), matching how
/// <see cref="HC.AI.MAPI.BL.LLMModel.ILLMModelBL"/> already separates "decide the model" from
/// "run the model."
/// </summary>
public interface IPersonaModelResolutionBL
{
    /// <summary>US012: resolve a persona's active Classification model + prompt.</summary>
    PersonaModelResolutionResult ResolveClassification(int roleId);

    /// <summary>
    /// US013: resolve the Executor model + prompt for a persona + the prompt-type code a
    /// Classification run produced. Returns <c>IsResolved = false</c> if the code doesn't map to
    /// a known type, or no active Executor config exists for that persona+type — the AC for both
    /// stories flags this fallback behavior as needing Product Owner sign-off; this method
    /// reports "not resolved" rather than silently deciding what happens next.
    /// </summary>
    PersonaModelResolutionResult ResolveExecutor(int roleId, string promptTypeCode);
}
