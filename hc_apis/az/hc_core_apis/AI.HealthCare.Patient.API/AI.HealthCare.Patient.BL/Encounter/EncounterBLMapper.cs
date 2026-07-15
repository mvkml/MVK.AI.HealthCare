using System.Globalization;
using AI.HealthCare.Patient.Models.Encounter;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's encounters.csv:
// Id,START,STOP,PATIENT,ORGANIZATION,PROVIDER,PAYER,ENCOUNTERCLASS,CODE,DESCRIPTION,BASE_ENCOUNTER_COST,TOTAL_CLAIM_COST,PAYER_COVERAGE,REASONCODE,REASONDESCRIPTION
public class EncounterBLMapper : IEncounterBLMapper
{
    private const int ExpectedColumnCount = 15;

    public EncounterItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new EncounterItem
        {
            Id = Guid.Parse(row[0]),
            Start = ParseDate(row[1]),
            Stop = ParseDate(row[2]),
            PatientId = Guid.Parse(row[3]),
            OrganizationId = Guid.Parse(row[4]),
            ProviderId = Guid.Parse(row[5]),
            PayerId = ParseGuid(row[6]),
            EncounterClass = NullIfEmpty(row[7]),
            Code = NullIfEmpty(row[8]),
            Description = NullIfEmpty(row[9]),
            BaseEncounterCost = ParseDecimal(row[10]),
            TotalClaimCost = ParseDecimal(row[11]),
            PayerCoverage = ParseDecimal(row[12]),
            ReasonCode = NullIfEmpty(row[13]),
            ReasonDescription = NullIfEmpty(row[14]),
        };
    }

    public EncounterItem ToItem(EncounterRequest request) => new()
    {
        Start = request.Start,
        Stop = request.Stop,
        PatientId = request.PatientId,
        OrganizationId = request.OrganizationId,
        ProviderId = request.ProviderId,
        PayerId = request.PayerId,
        EncounterClass = request.EncounterClass,
        Code = request.Code,
        Description = request.Description,
        BaseEncounterCost = request.BaseEncounterCost,
        TotalClaimCost = request.TotalClaimCost,
        PayerCoverage = request.PayerCoverage,
        ReasonCode = request.ReasonCode,
        ReasonDescription = request.ReasonDescription
    };

    public EncounterResponse ToResponse(EncounterItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        OrganizationId = item.OrganizationId,
        ProviderId = item.ProviderId,
        PayerId = item.PayerId,
        EncounterClass = item.EncounterClass,
        Code = item.Code,
        Description = item.Description,
        BaseEncounterCost = item.BaseEncounterCost,
        TotalClaimCost = item.TotalClaimCost,
        PayerCoverage = item.PayerCoverage,
        ReasonCode = item.ReasonCode,
        ReasonDescription = item.ReasonDescription
    };

    private static DateTime? ParseDate(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

    private static Guid? ParseGuid(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : Guid.Parse(value);

    private static decimal? ParseDecimal(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : decimal.Parse(value, CultureInfo.InvariantCulture);

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
