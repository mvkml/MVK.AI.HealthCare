using System.Globalization;
using AI.HealthCare.Patient.Models.Medication;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's medications.csv:
// START,STOP,PATIENT,PAYER,ENCOUNTER,CODE,DESCRIPTION,BASE_COST,PAYER_COVERAGE,DISPENSES,TOTALCOST,REASONCODE,REASONDESCRIPTION
public class MedicationBLMapper : IMedicationBLMapper
{
    private const int ExpectedColumnCount = 13;

    public MedicationItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new MedicationItem
        {
            Start = ParseDate(row[0]),
            Stop = ParseDate(row[1]),
            PatientId = Guid.Parse(row[2]),
            PayerId = ParseGuid(row[3]),
            EncounterId = Guid.Parse(row[4]),
            Code = NullIfEmpty(row[5]),
            Description = NullIfEmpty(row[6]),
            BaseCost = ParseDecimal(row[7]),
            PayerCoverage = ParseDecimal(row[8]),
            Dispenses = ParseInt(row[9]),
            TotalCost = ParseDecimal(row[10]),
            ReasonCode = NullIfEmpty(row[11]),
            ReasonDescription = NullIfEmpty(row[12]),
        };
    }

    public MedicationItem ToItem(MedicationRequest request) => new()
    {
        Start = request.Start,
        Stop = request.Stop,
        PatientId = request.PatientId,
        PayerId = request.PayerId,
        EncounterId = request.EncounterId,
        Code = request.Code,
        Description = request.Description,
        BaseCost = request.BaseCost,
        PayerCoverage = request.PayerCoverage,
        TotalCost = request.TotalCost,
        Dispenses = request.Dispenses,
        ReasonCode = request.ReasonCode,
        ReasonDescription = request.ReasonDescription
    };

    public MedicationResponse ToResponse(MedicationItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        PayerId = item.PayerId,
        EncounterId = item.EncounterId,
        Code = item.Code,
        Description = item.Description,
        BaseCost = item.BaseCost,
        PayerCoverage = item.PayerCoverage,
        TotalCost = item.TotalCost,
        Dispenses = item.Dispenses,
        ReasonCode = item.ReasonCode,
        ReasonDescription = item.ReasonDescription
    };

    private static DateTime? ParseDate(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

    private static Guid? ParseGuid(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : Guid.Parse(value);

    private static decimal? ParseDecimal(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : decimal.Parse(value, CultureInfo.InvariantCulture);

    private static int? ParseInt(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : int.Parse(value, CultureInfo.InvariantCulture);

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
