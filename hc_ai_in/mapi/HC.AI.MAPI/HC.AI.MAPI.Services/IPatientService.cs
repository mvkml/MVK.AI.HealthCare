using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.Services;

public interface IPatientService
{
    /// <summary>
    /// Same contract as <see cref="IDoctorService.ProvidePromptAsync"/>, Patient-side. Only the
    /// locked/structured-request shape is implemented — Patient has no equivalent of Doctor's
    /// older demo methods (<c>HandleRequestAsync</c>/<c>BasicHandleRequestAsync</c>/
    /// <c>GetChatResponseByPrompt</c>), since those predate ADR001's PromptModel pattern and
    /// aren't part of the feature being mirrored here.
    /// </summary>
    Task<PromptResponse> ProvidePromptAsync(PromptModel model);
}
