using HC.AI.MAPI.Models.Persona;

namespace HC.AI.MAPI.BL.Persona;

/// <summary>
/// Reads persona/prompt-type/model config — today from an in-memory mock (see
/// <see cref="PersonaLlmConfigMockProvider"/>), later from the DB tables proposed in
/// hc_agile/architecture/design_patterns/persona_dynamic_llm_prompt_schema.md once those open
/// design questions are signed off (US011). Consumers depend on this interface, not the mock
/// directly, so swapping in the real DB-backed provider later is additive — same pattern as
/// AuthMockService -> AuthService on the Angular side.
/// </summary>
public interface IPersonaLlmConfigProvider
{
    PersonaLlmOption? GetActiveOption(int roleId, string modelRole, int? personaPromptTypeId);
    PersonaPromptRecord? GetActivePrompt(int roleId, string modelRole, int? personaPromptTypeId);
    PersonaPromptType? GetPromptTypeByCode(int roleId, string code);
}
