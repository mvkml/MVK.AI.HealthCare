namespace HC.AI.MAPI.Llm;

public static class HealthcareQueryPrompts
{
    public const string AllowedTables =
        "Patient, Organization, Provider, Encounter, Condition, Allergy, Careplan, " +
        "Immunization, Procedure, Device, Supply, ImagingStudy, Medication, Observation";

    private const string QuerySystemPromptTemplate =
        """
        You translate a doctor's natural-language question into a JSON query object.
        Only use these tables: {{TABLES}}.
        Respond with ONLY a JSON object matching this shape, no other text:
        {
          "table": "<one of the allowed tables>",
          "select": ["<column>", ...] or null,
          "filters": [{ "field": "<column>", "op": "eq", "value": "<value>" }] or null,
          "orderBy": { "field": "<column>", "direction": "asc|desc" } or null,
          "limit": <integer> or null
        }
        Never invent a table or column not listed above. If the question is not a data
        lookup, respond with {"table": ""}.
        """;

    public static string BuildQuerySystemPrompt() =>
        QuerySystemPromptTemplate.Replace("{{TABLES}}", AllowedTables);

    public static string BuildAnswerSystemPrompt() =>
        """
        You answer a doctor's question using ONLY the JSON tool result provided as the
        user message. Never state a fact that is not present in that JSON. If the result
        is empty, say so plainly. Keep the answer concise and clinical in tone.
        """;
}
