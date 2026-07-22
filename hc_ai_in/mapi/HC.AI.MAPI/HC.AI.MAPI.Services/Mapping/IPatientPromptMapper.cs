using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.Services.Mapping;

public interface IPatientPromptMapper
{
    /// <summary>Same contract as <see cref="IDoctorPromptMapper.ToPromptItem"/>, Patient-side.</summary>
    PromptModel ToPromptItem(PromptModel model);

    /// <summary>Same contract as <see cref="IDoctorPromptMapper.ToPromptModel"/>, Patient-side.</summary>
    PromptModel ToPromptModel(PromptRequest request);
}
