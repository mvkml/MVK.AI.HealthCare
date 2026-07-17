using HC.AI.MAPI.Models;

namespace HC.AI.MAPI.Tool;

public class HealthcareQueryTool
{
    private readonly PatientApiClient _patientApiClient;

    public HealthcareQueryTool(PatientApiClient patientApiClient)
    {
        _patientApiClient = patientApiClient;
    }

    public Task<QueryResult> ExecuteQueryAsync(QueryRequest request)
    {
        throw new NotImplementedException();
    }
}
