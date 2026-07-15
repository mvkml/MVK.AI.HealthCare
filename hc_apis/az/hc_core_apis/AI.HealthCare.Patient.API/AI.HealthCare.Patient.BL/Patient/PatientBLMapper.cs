using System.Globalization;
using AI.HealthCare.Patient.Models.Patient;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's patients.csv:
// Id,BIRTHDATE,DEATHDATE,SSN,DRIVERS,PASSPORT,PREFIX,FIRST,MIDDLE,LAST,SUFFIX,MAIDEN,MARITAL,RACE,
// ETHNICITY,GENDER,BIRTHPLACE,ADDRESS,CITY,STATE,COUNTY,FIPS,ZIP,LAT,LON,HEALTHCARE_EXPENSES,
// HEALTHCARE_COVERAGE,INCOME
public class PatientBLMapper : IPatientBLMapper
{
    private const int ExpectedColumnCount = 28;

    public PatientItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new PatientItem
        {
            Id = Guid.Parse(row[0]),
            BirthDate = DateTime.Parse(row[1], CultureInfo.InvariantCulture),
            DeathDate = ParseDate(row[2]),
            Ssn = NullIfEmpty(row[3]),
            Drivers = NullIfEmpty(row[4]),
            Passport = NullIfEmpty(row[5]),
            Prefix = NullIfEmpty(row[6]),
            First = row[7],
            Middle = NullIfEmpty(row[8]),
            Last = row[9],
            Suffix = NullIfEmpty(row[10]),
            Maiden = NullIfEmpty(row[11]),
            Marital = NullIfEmpty(row[12]),
            Race = NullIfEmpty(row[13]),
            Ethnicity = NullIfEmpty(row[14]),
            Gender = NullIfEmpty(row[15]),
            Birthplace = NullIfEmpty(row[16]),
            Address = NullIfEmpty(row[17]),
            City = NullIfEmpty(row[18]),
            State = NullIfEmpty(row[19]),
            County = NullIfEmpty(row[20]),
            Fips = NullIfEmpty(row[21]),
            Zip = NullIfEmpty(row[22]),
            Lat = ParseDecimal(row[23]),
            Lon = ParseDecimal(row[24]),
            HealthcareExpenses = ParseDecimal(row[25]),
            HealthcareCoverage = ParseDecimal(row[26]),
            Income = ParseInt(row[27]),
        };
    }

    public PatientItem ToItem(PatientRequest request) => new()
    {
        BirthDate = request.BirthDate,
        DeathDate = request.DeathDate,
        Ssn = request.Ssn,
        Drivers = request.Drivers,
        Passport = request.Passport,
        Prefix = request.Prefix,
        First = request.First,
        Middle = request.Middle,
        Last = request.Last,
        Suffix = request.Suffix,
        Maiden = request.Maiden,
        Marital = request.Marital,
        Race = request.Race,
        Ethnicity = request.Ethnicity,
        Gender = request.Gender,
        Birthplace = request.Birthplace,
        Address = request.Address,
        City = request.City,
        State = request.State,
        County = request.County,
        Fips = request.Fips,
        Zip = request.Zip,
        Lat = request.Lat,
        Lon = request.Lon,
        HealthcareExpenses = request.HealthcareExpenses,
        HealthcareCoverage = request.HealthcareCoverage,
        Income = request.Income
    };

    public PatientResponse ToResponse(PatientItem item, bool includePii) => new()
    {
        Id = item.Id,
        Ssn = includePii ? Mask(item.Ssn) : null,
        Drivers = includePii ? Mask(item.Drivers) : null,
        Passport = includePii ? Mask(item.Passport) : null,
        BirthDate = item.BirthDate,
        DeathDate = item.DeathDate,
        Prefix = item.Prefix,
        First = item.First,
        Middle = item.Middle,
        Last = item.Last,
        Suffix = item.Suffix,
        Maiden = item.Maiden,
        Marital = item.Marital,
        Race = item.Race,
        Ethnicity = item.Ethnicity,
        Gender = item.Gender,
        Birthplace = item.Birthplace,
        Address = item.Address,
        City = item.City,
        State = item.State,
        County = item.County,
        Fips = item.Fips,
        Zip = item.Zip,
        Lat = item.Lat,
        Lon = item.Lon,
        HealthcareExpenses = item.HealthcareExpenses,
        HealthcareCoverage = item.HealthcareCoverage,
        Income = item.Income
    };

    /// <summary>Masks all but the last 4 characters, e.g. "999-83-4938" -> "***-**-4938". For demo/learning purposes only — not a substitute for real PII encryption at rest.</summary>
    private static string? Mask(string? value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        if (value.Length <= 4) return new string('*', value.Length);

        var visible = value[^4..];
        var maskedPrefix = value[..^4].Select(c => c == '-' ? '-' : '*');
        return new string(maskedPrefix.ToArray()) + visible;
    }

    private static DateTime? ParseDate(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : DateTime.Parse(value, CultureInfo.InvariantCulture);

    private static decimal? ParseDecimal(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : decimal.Parse(value, CultureInfo.InvariantCulture);

    private static int? ParseInt(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : int.Parse(value, CultureInfo.InvariantCulture);

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
