namespace HC.AI.MAPI.Models.Persona;

/// <summary>
/// Mirrors the proposed <c>PersonaPromptType</c> table (EPIC001 schema doc) — the "type of
/// request" a Classification model routes to, scoped per persona (<see cref="RoleId"/>).
/// Mock-only today: no DB table exists yet, real values are unconfirmed (see
/// PersonaLlmConfigMockProvider for why <see cref="Code"/> below is a placeholder, not a
/// business decision).
/// </summary>
public class PersonaPromptType
{
    public int PersonaPromptTypeId { get; set; }
    public int RoleId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
