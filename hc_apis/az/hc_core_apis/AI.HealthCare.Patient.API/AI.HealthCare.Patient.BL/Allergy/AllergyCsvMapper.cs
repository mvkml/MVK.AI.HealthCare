using System.Globalization;
using AI.HealthCare.Patient.Models.Allergy;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's allergies.csv:
// START,STOP,PATIENT,ENCOUNTER,CODE,SYSTEM,DESCRIPTION,TYPE,CATEGORY,REACTION1,DESCRIPTION1,SEVERITY1,REACTION2,DESCRIPTION2,SEVERITY2
public class AllergyCsvMapper : IAllergyCsvMapper
{
    private const int ExpectedColumnCount = 15;

    public AllergyItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new AllergyItem
        {
            Start = ParseDate(row[0]),
            Stop = ParseDate(row[1]),
            PatientId = Guid.Parse(row[2]),
            EncounterId = Guid.Parse(row[3]),
            Code = NullIfEmpty(row[4]),
            System = NullIfEmpty(row[5]),
            Description = NullIfEmpty(row[6]),
            Type = NullIfEmpty(row[7]),
            Category = NullIfEmpty(row[8]),
            Reaction1 = NullIfEmpty(row[9]),
            Description1 = NullIfEmpty(row[10]),
            Severity1 = NullIfEmpty(row[11]),
            Reaction2 = NullIfEmpty(row[12]),
            Description2 = NullIfEmpty(row[13]),
            Severity2 = NullIfEmpty(row[14]),
        };
    }

    private static DateTime? ParseDate(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : DateTime.Parse(value, CultureInfo.InvariantCulture);

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;

    public AllergyItem ToItem(AllergyRequest request) => new()
    {
        Start = request.Start,
        Stop = request.Stop,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        Code = request.Code,
        System = request.System,
        Description = request.Description,
        Type = request.Type,
        Category = request.Category,
        Reaction1 = request.Reaction1,
        Description1 = request.Description1,
        Severity1 = request.Severity1,
        Reaction2 = request.Reaction2,
        Description2 = request.Description2,
        Severity2 = request.Severity2
    };

    public AllergyResponse ToResponse(AllergyItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        Code = item.Code,
        System = item.System,
        Description = item.Description,
        Type = item.Type,
        Category = item.Category,
        Reaction1 = item.Reaction1,
        Description1 = item.Description1,
        Severity1 = item.Severity1,
        Reaction2 = item.Reaction2,
        Description2 = item.Description2,
        Severity2 = item.Severity2
    };
}
