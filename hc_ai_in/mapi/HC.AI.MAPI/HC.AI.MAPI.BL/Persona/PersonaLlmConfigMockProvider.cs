using HC.AI.MAPI.Common;
using HC.AI.MAPI.Models.Persona;

namespace HC.AI.MAPI.BL.Persona;

/// <summary>
/// In-memory stand-in for the three tables proposed in
/// hc_agile/architecture/design_patterns/persona_dynamic_llm_prompt_schema.md
/// (PersonaPromptType, PersonaLLMOption, PersonaPrompt). No DB table exists yet — that schema
/// doc lists open questions (single- vs multi-role-per-user, real prompt-type values, fallback
/// semantics) that need sign-off before real DDL. This mock exists so the Classification ->
/// Executor resolution mechanism (US012/US013) can be built and demonstrated now instead of
/// blocking on that sign-off, per the user's "implement with the mock models" instruction.
///
/// Only Doctor (RoleId 1) is seeded. Only ONE prompt-type ("General") exists — a deliberate
/// placeholder, not a business decision: the schema doc explicitly says "not guessing at these
/// [prompt-type values] — needs your input before this table can be seeded." A single catch-all
/// type is the minimum needed to prove the Classification -> Executor handoff works end to end;
/// it is not a proposal for what Doctor's real prompt-types should be.
/// </summary>
public class PersonaLlmConfigMockProvider : IPersonaLlmConfigProvider
{
    private const int DoctorRoleId = 1;

    private static readonly List<PersonaPromptType> PromptTypes =
    [
        new PersonaPromptType
        {
            PersonaPromptTypeId = 1,
            RoleId = DoctorRoleId,
            Code = "General",
            Name = "General Inquiry (placeholder — real Doctor prompt-types not yet defined)"
        }
    ];

    private static readonly List<PersonaLlmOption> LlmOptions =
    [
        new PersonaLlmOption
        {
            PersonaLlmOptionId = 1,
            RoleId = DoctorRoleId,
            ModelRole = ModelRole.Classification,
            PersonaPromptTypeId = null,
            ModelName = "qwen2.5:7b",
            Provider = APIConstants.OllammaProvideName,
            IsDefault = true,
            Priority = 1
        },
        new PersonaLlmOption
        {
            PersonaLlmOptionId = 2,
            RoleId = DoctorRoleId,
            ModelRole = ModelRole.Executor,
            PersonaPromptTypeId = 1,
            ModelName = "hc-doctor-executor:latest",
            Provider = APIConstants.OllammaProvideName,
            IsDefault = true,
            Priority = 1
        }
    ];

    private static readonly List<PersonaPromptRecord> Prompts =
    [
        new PersonaPromptRecord
        {
            PersonaPromptId = 1,
            RoleId = DoctorRoleId,
            ModelRole = ModelRole.Classification,
            PersonaPromptTypeId = null,
            PromptText =
                "You are a request router for a healthcare assistant. Read the Doctor's message " +
                "and respond with exactly one prompt-type code. Today only \"General\" exists.",
            Version = 1
        },
        new PersonaPromptRecord
        {
            PersonaPromptId = 2,
            RoleId = DoctorRoleId,
            ModelRole = ModelRole.Executor,
            PersonaPromptTypeId = 1,
            PromptText =
                "You are a clinical assistant for a Doctor. Answer using only information the " +
                "Doctor provides or that comes from a tool result — never fabricate patient data.",
            Version = 1
        }
    ];

    public PersonaLlmOption? GetActiveOption(int roleId, string modelRole, int? personaPromptTypeId) =>
        LlmOptions.FirstOrDefault(o =>
            o.RoleId == roleId &&
            o.ModelRole == modelRole &&
            o.PersonaPromptTypeId == personaPromptTypeId &&
            o.IsActive);

    public PersonaPromptRecord? GetActivePrompt(int roleId, string modelRole, int? personaPromptTypeId) =>
        Prompts.FirstOrDefault(p =>
            p.RoleId == roleId &&
            p.ModelRole == modelRole &&
            p.PersonaPromptTypeId == personaPromptTypeId &&
            p.IsActive);

    public PersonaPromptType? GetPromptTypeByCode(int roleId, string code) =>
        PromptTypes.FirstOrDefault(t =>
            t.RoleId == roleId &&
            string.Equals(t.Code, code, StringComparison.OrdinalIgnoreCase) &&
            t.IsActive);
}
