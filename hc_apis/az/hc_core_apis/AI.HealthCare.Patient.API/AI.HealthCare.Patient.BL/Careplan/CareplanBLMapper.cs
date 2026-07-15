using System.Globalization;
using AI.HealthCare.Patient.Models.Careplan;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's careplans.csv:
// Id,START,STOP,PATIENT,ENCOUNTER,CODE,DESCRIPTION,REASONCODE,REASONDESCRIPTION
public class CareplanBLMapper : ICareplanBLMapper
{
    private const int ExpectedColumnCount = 9;

    public CareplanItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new CareplanItem
        {
            Id = Guid.Parse(row[0]),
            Start = ParseDate(row[1]),
            Stop = ParseDate(row[2]),
            PatientId = Guid.Parse(row[3]),
            EncounterId = Guid.Parse(row[4]),
            Code = NullIfEmpty(row[5]),
            Description = NullIfEmpty(row[6]),
            ReasonCode = NullIfEmpty(row[7]),
            ReasonDescription = NullIfEmpty(row[8]),
        };
    }

    public CareplanItem ToItem(CareplanRequest request) => new()
    {
        Start = request.Start,
        Stop = request.Stop,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        Code = request.Code,
        Description = request.Description,
        ReasonCode = request.ReasonCode,
        ReasonDescription = request.ReasonDescription
    };

    public CareplanResponse ToResponse(CareplanItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        Code = item.Code,
        Description = item.Description,
        ReasonCode = item.ReasonCode,
        ReasonDescription = item.ReasonDescription
    };

    private static DateTime? ParseDate(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : DateTime.Parse(value, CultureInfo.InvariantCulture);

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
