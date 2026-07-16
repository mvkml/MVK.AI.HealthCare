using System.Globalization;
using AI.HealthCare.Patient.Models.Observation;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's observations.csv:
// DATE,PATIENT,ENCOUNTER,CATEGORY,CODE,DESCRIPTION,VALUE,UNITS,TYPE
public class ObservationBLMapper : IObservationBLMapper
{
    private const int ExpectedColumnCount = 9;

    public ObservationItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new ObservationItem
        {
            Date = DateTime.Parse(row[0], CultureInfo.InvariantCulture),
            PatientId = Guid.Parse(row[1]),
            EncounterId = ParseNullableGuid(row[2]),
            Category = NullIfEmpty(row[3]),
            Code = NullIfEmpty(row[4]),
            Description = NullIfEmpty(row[5]),
            Value = NullIfEmpty(row[6]),
            Units = NullIfEmpty(row[7]),
            Type = NullIfEmpty(row[8]),
        };
    }

    public ObservationItem ToItem(ObservationRequest request) => new()
    {
        Date = request.Date,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        Category = request.Category,
        Code = request.Code,
        Description = request.Description,
        Value = request.Value,
        Units = request.Units,
        Type = request.Type
    };

    public ObservationResponse ToResponse(ObservationItem item) => new()
    {
        Id = item.Id,
        Date = item.Date,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        Category = item.Category,
        Code = item.Code,
        Description = item.Description,
        Value = item.Value,
        Units = item.Units,
        Type = item.Type
    };

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;

    // Patient-level yearly summary scores (DALY/QALY/QOLS) have no ENCOUNTER in the source data.
    private static Guid? ParseNullableGuid(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : Guid.Parse(value);
}
