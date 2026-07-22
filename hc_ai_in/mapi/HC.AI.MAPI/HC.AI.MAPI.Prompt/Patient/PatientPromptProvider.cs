namespace HC.AI.MAPI.Prompt.Patient;

/// <summary>
/// Patient persona's system prompt (US010/PB024 real-backend follow-up). Deliberately more
/// cautious than <see cref="HC.AI.MAPI.Prompt.Doctor.DoctorPromptProvider"/>'s clinical/concise
/// framing — a Patient-facing assistant should not sound like it's diagnosing, and should point
/// back to a real clinician for anything beyond general information. No Kernel-building
/// constructor here (unlike DoctorPromptProvider) since Patient has no equivalent of the
/// unused/legacy <c>GetChatResponseByPrompt</c> demo method — only the system prompt is needed
/// for the locked <c>PatientService.ProvidePromptAsync</c> path.
/// </summary>
public class PatientPromptProvider : IPatientPromptProvider
{
    private const string SystemPrompt =
        "You are a supportive healthcare assistant helping a patient understand general health " +
        "information. Speak in plain, simple language — avoid clinical jargon. Do not diagnose, " +
        "prescribe, or state a definitive medical conclusion; for anything beyond general " +
        "information, clearly encourage the patient to follow up with their doctor. Be warm, " +
        "concise, and reassuring without minimizing real concerns.";

    public string GetSystemPrompt() => SystemPrompt;
}
