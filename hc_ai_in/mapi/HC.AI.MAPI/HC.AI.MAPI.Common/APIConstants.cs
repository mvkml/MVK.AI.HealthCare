namespace HC.AI.MAPI.Common
{
    public class APIConstants
    {
        public const string API_VERSION = "v1";
        public const string OllammaProvideName = "Ollama";
        public const string OpenAIProvideName = "OpenAI";
        public const string AzureOpenAIProvideName = "AzureOpenAI";
        public const string HuggingFaceProvideName = "HuggingFace";

        public const string DoctorPersonaName = "Doctor";
        public const string DoctorExecutorPersonaName = "HCDocExecutor";
        public const string DoctorClasificationPersonaName = "Doctor";
        public const string DoctorChatPromptKey = "doctor-chat";
        public const string DoctorServiceSourceName = "DoctorService";

        public const string PatientPersonaName = "Patient";
        public const string PatientExecutorPersonaName = "HCPatientExecutor";
        public const string PatientChatPromptKey = "patient-chat";
        public const string PatientServiceSourceName = "PatientService";

    }
}
