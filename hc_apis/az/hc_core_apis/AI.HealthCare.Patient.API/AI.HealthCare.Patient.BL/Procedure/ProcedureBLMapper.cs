using System.Globalization;
using AI.HealthCare.Patient.Models.Procedure;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's procedures.csv:
// START,STOP,PATIENT,ENCOUNTER,SYSTEM,CODE,DESCRIPTION,BASE_COST,REASONCODE,REASONDESCRIPTION
public class ProcedureBLMapper : IProcedureBLMapper
{
    private const int ExpectedColumnCount = 10;

    public ProcedureItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new ProcedureItem
        {
            Start = ParseDate(row[0]),
            Stop = ParseDate(row[1]),
            PatientId = Guid.Parse(row[2]),
            EncounterId = Guid.Parse(row[3]),
            System = NullIfEmpty(row[4]),
            Code = NullIfEmpty(row[5]),
            Description = NullIfEmpty(row[6]),
            BaseCost = ParseDecimal(row[7]),
            ReasonCode = NullIfEmpty(row[8]),
            ReasonDescription = NullIfEmpty(row[9]),
        };
    }

    public ProcedureItem ToItem(ProcedureRequest request) => new()
    {
        Start = request.Start,
        Stop = request.Stop,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        System = request.System,
        Code = request.Code,
        Description = request.Description,
        BaseCost = request.BaseCost,
        ReasonCode = request.ReasonCode,
        ReasonDescription = request.ReasonDescription
    };

    public ProcedureResponse ToResponse(ProcedureItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        System = item.System,
        Code = item.Code,
        Description = item.Description,
        BaseCost = item.BaseCost,
        ReasonCode = item.ReasonCode,
        ReasonDescription = item.ReasonDescription
    };

    private static DateTime? ParseDate(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

    private static decimal? ParseDecimal(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : decimal.Parse(value, CultureInfo.InvariantCulture);

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
