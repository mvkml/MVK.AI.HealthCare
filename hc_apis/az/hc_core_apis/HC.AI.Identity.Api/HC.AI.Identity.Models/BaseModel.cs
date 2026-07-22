namespace HC.AI.Identity.Models;

/// <summary>
/// Common base class for all API models. Carries the result status
/// and message that every response model should expose.
/// </summary>
public class BaseModel
{
    /// <summary>True if the request/data is invalid; false if valid.</summary>
    public bool IsNotValid { get; set; }

    /// <summary>Human-readable message describing the result or error.</summary>
    public string Message { get; set; } = string.Empty;
}
