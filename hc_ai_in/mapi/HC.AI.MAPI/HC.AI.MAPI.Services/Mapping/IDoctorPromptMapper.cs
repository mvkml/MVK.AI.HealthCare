using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.Services.Mapping;

public interface IDoctorPromptMapper
{
    /// <summary>
    /// Per ADR001, takes and returns the full <see cref="PromptModel"/>. Requires
    /// <c>model.PromptRequest</c> to already be set; populates <c>model.PromptItem</c>,
    /// including copying over Message and the generation parameters — except
    /// <c>LLMOptions</c>/<c>LLMProvider</c>, which is a business decision set by
    /// <c>HC.AI.MAPI.BL.LLMModel.ILLMModelBL.GetModelDetails</c>, not this mapper. This is the
    /// last place <c>PromptRequest</c> is read — every layer downstream of here uses
    /// <c>PromptItem</c> exclusively.
    /// </summary>
    PromptModel ToPromptItem(PromptModel model);

    /// <summary>
    /// The one construction-point exception in ADR001: no <see cref="PromptModel"/> exists yet
    /// at this call, so it's built from a raw <see cref="PromptRequest"/> instead. Internally
    /// delegates to <see cref="ToPromptItem"/> to populate the item, so there is a single place
    /// that builds one.
    /// </summary>
    PromptModel ToPromptModel(PromptRequest request);
}
