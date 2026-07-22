namespace HC.AI.MAPI.Models.Persona;

/// <summary>
/// The two model roles a persona can configure, per the EPIC001 schema proposal
/// (hc_agile/architecture/design_patterns/persona_dynamic_llm_prompt_schema.md).
/// </summary>
public static class ModelRole
{
    public const string Classification = "Classification";
    public const string Executor = "Executor";
}
