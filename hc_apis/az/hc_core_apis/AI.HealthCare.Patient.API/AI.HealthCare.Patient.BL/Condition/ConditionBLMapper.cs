using System.Globalization;
using AI.HealthCare.Patient.Models.Condition;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's conditions.csv:
// START,STOP,PATIENT,ENCOUNTER,SYSTEM,CODE,DESCRIPTION
public class ConditionBLMapper : IConditionBLMapper
{
    private const int ExpectedColumnCount = 7;

    public ConditionItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new ConditionItem
        {
            Start = ParseDate(row[0]),
            Stop = ParseDate(row[1]),
            PatientId = Guid.Parse(row[2]),
            EncounterId = Guid.Parse(row[3]),
            System = NullIfEmpty(row[4]),
            Code = NullIfEmpty(row[5]),
            Description = NullIfEmpty(row[6]),
        };
    }

    public ConditionItem ToItem(ConditionRequest request) => new()
    {
        Start = request.Start,
        Stop = request.Stop,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        System = request.System,
        Code = request.Code,
        Description = request.Description
    };

    public ConditionResponse ToResponse(ConditionItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        System = item.System,
        Code = item.Code,
        Description = item.Description
    };

    private static DateTime? ParseDate(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : DateTime.Parse(value, CultureInfo.InvariantCulture);

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
