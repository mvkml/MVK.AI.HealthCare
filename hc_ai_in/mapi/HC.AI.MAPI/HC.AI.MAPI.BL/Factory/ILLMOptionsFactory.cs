using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.BL.Factory;

public interface ILLMOptionsFactory
{
    /// <summary>
    /// Binds and returns the <see cref="LLMOptions"/> for the given appsettings.json section
    /// name (e.g. "Ollama", "HCClassification", "HCDocExecutor"). Named <c>persona</c> since
    /// each section currently maps 1:1 to a persona/role (see APIConstants.*PersonaName).
    /// </summary>
    LLMOptions GetLLMOptions(string persona);
}
