using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.BL.LLMModel;

public interface ILLMModelBL
{
    /// <summary>
    /// Decides which LLM/model/provider to use, keyed by <c>model.PromptItem.Persona</c> (user
    /// type: Doctor, Insurance Provider, Client, etc.) — ultimately provider-per-persona (Ollama
    /// vs OpenAI, etc.) once database-backed. See PB019 in the backlog — not implemented yet.
    /// Per ADR001, takes and returns the full <see cref="PromptModel"/> envelope.
    /// Requires <c>model.PromptItem</c> to already be set; populates <c>model.LLMOptions</c> and
    /// <c>model.PromptItem.LLMProvider</c> — this is the single place that decides the provider,
    /// so the two never drift apart.
    /// </summary>
    PromptModel GetModelDetails(PromptModel model);
}
