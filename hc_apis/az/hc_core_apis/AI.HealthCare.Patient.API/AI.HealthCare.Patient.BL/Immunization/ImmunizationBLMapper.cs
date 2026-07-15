using System.Globalization;
using AI.HealthCare.Patient.Models.Immunization;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's immunizations.csv:
// DATE,PATIENT,ENCOUNTER,CODE,DESCRIPTION,BASE_COST
public class ImmunizationBLMapper : IImmunizationBLMapper
{
    private const int ExpectedColumnCount = 6;

    public ImmunizationItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new ImmunizationItem
        {
            Date = DateTime.Parse(row[0], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
            PatientId = Guid.Parse(row[1]),
            EncounterId = Guid.Parse(row[2]),
            Code = NullIfEmpty(row[3]),
            Description = NullIfEmpty(row[4]),
            BaseCost = ParseDecimal(row[5]),
        };
    }

    public ImmunizationItem ToItem(ImmunizationRequest request) => new()
    {
        Date = request.Date,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        Code = request.Code,
        Description = request.Description,
        BaseCost = request.BaseCost
    };

    public ImmunizationResponse ToResponse(ImmunizationItem item) => new()
    {
        Id = item.Id,
        Date = item.Date,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        Code = item.Code,
        Description = item.Description,
        BaseCost = item.BaseCost
    };

    private static decimal? ParseDecimal(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : decimal.Parse(value, CultureInfo.InvariantCulture);

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
