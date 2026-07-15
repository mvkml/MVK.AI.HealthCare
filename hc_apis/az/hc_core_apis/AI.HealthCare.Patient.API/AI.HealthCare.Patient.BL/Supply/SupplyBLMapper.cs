using System.Globalization;
using AI.HealthCare.Patient.Models.Supply;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's supplies.csv:
// DATE,PATIENT,ENCOUNTER,CODE,DESCRIPTION,QUANTITY
public class SupplyBLMapper : ISupplyBLMapper
{
    private const int ExpectedColumnCount = 6;

    public SupplyItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new SupplyItem
        {
            Date = DateTime.Parse(row[0], CultureInfo.InvariantCulture),
            PatientId = Guid.Parse(row[1]),
            EncounterId = Guid.Parse(row[2]),
            Code = NullIfEmpty(row[3]),
            Description = NullIfEmpty(row[4]),
            Quantity = ParseInt(row[5]),
        };
    }

    public SupplyItem ToItem(SupplyRequest request) => new()
    {
        Date = request.Date,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        Code = request.Code,
        Description = request.Description,
        Quantity = request.Quantity
    };

    public SupplyResponse ToResponse(SupplyItem item) => new()
    {
        Id = item.Id,
        Date = item.Date,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        Code = item.Code,
        Description = item.Description,
        Quantity = item.Quantity
    };

    private static int? ParseInt(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : int.Parse(value, CultureInfo.InvariantCulture);

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
