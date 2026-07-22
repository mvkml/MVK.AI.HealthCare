using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.BL.LLMModel;

public interface ILLMModelBL
{
    /// <summary>
    /// Decides which LLM/model/provider to use, keyed by <c>model.PromptItem.ModelKey</c> (the
    /// appsettings.json section name, derived from <c>PromptItem.Persona</c> by
    /// <c>DoctorPromptMapper</c> — e.g. "HCDocExecutor" for Doctor, "HCPatientExecutor" for
    /// Patient) — config-based persona branching today; database-backed provider-per-persona
    /// (Ollama vs OpenAI, etc.) is PB019/PB032/EPIC001, not implemented yet.
    /// Per ADR001, takes and returns the full <see cref="PromptModel"/> envelope.
    /// Requires <c>model.PromptItem</c> to already be set; populates <c>model.LLMOptions</c> and
    /// <c>model.PromptItem.LLMProvider</c> — this is the single place that decides the provider,
    /// so the two never drift apart.
    /// </summary>
    PromptModel GetModelDetails(PromptModel model);
}
