using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.AL;

public interface IPatientSemanticProcess
{
    /// <summary>Same contract as <see cref="IDoctorSemanticProcess.ProcessAsync"/>, Patient-side.</summary>
    Task<PromptModel> ProcessAsync(PromptModel model);
}
