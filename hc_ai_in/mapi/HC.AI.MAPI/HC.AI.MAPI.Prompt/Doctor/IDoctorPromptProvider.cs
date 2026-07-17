namespace HC.AI.MAPI.Prompt.Doctor;

public interface IDoctorPromptProvider
{
    string GetSystemPrompt();
    Task<string> GetChatResponseByPrompt(string message);
}
