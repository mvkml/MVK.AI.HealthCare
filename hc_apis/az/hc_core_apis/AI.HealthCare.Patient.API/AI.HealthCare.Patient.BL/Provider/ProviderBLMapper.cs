using System.Globalization;
using AI.HealthCare.Patient.Models.Provider;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's providers.csv:
// Id,ORGANIZATION,NAME,GENDER,SPECIALITY,ADDRESS,CITY,STATE,ZIP,LAT,LON,ENCOUNTERS,PROCEDURES
public class ProviderBLMapper : IProviderBLMapper
{
    private const int ExpectedColumnCount = 13;

    public ProviderItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new ProviderItem
        {
            Id = Guid.Parse(row[0]),
            OrganizationId = Guid.Parse(row[1]),
            Name = row[2],
            Gender = NullIfEmpty(row[3]),
            Speciality = NullIfEmpty(row[4]),
            Address = NullIfEmpty(row[5]),
            City = NullIfEmpty(row[6]),
            State = NullIfEmpty(row[7]),
            Zip = NullIfEmpty(row[8]),
            Lat = ParseDecimal(row[9]),
            Lon = ParseDecimal(row[10]),
            Encounters = ParseInt(row[11]),
            Procedures = ParseInt(row[12]),
        };
    }

    public ProviderItem ToItem(ProviderRequest request) => new()
    {
        OrganizationId = request.OrganizationId,
        Name = request.Name,
        Gender = request.Gender,
        Speciality = request.Speciality,
        Address = request.Address,
        City = request.City,
        State = request.State,
        Zip = request.Zip,
        Lat = request.Lat,
        Lon = request.Lon,
        Encounters = request.Encounters,
        Procedures = request.Procedures
    };

    public ProviderResponse ToResponse(ProviderItem item) => new()
    {
        Id = item.Id,
        OrganizationId = item.OrganizationId,
        Name = item.Name,
        Gender = item.Gender,
        Speciality = item.Speciality,
        Address = item.Address,
        City = item.City,
        State = item.State,
        Zip = item.Zip,
        Lat = item.Lat,
        Lon = item.Lon,
        Encounters = item.Encounters,
        Procedures = item.Procedures
    };

    private static decimal? ParseDecimal(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : decimal.Parse(value, CultureInfo.InvariantCulture);

    private static int? ParseInt(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : int.Parse(value, CultureInfo.InvariantCulture);

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
