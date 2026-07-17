namespace HC.AI.MAPI.AL;

public interface IDoctorAgent
{
    Task<string> HandleRequestAsync(string message);
     Task<string> BasicHandleRequestAsync(string message);
}
