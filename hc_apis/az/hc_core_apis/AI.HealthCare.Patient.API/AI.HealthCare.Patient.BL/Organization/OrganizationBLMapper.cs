using System.Globalization;
using AI.HealthCare.Patient.Models.Organization;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's organizations.csv:
// Id,NAME,ADDRESS,CITY,STATE,ZIP,LAT,LON,PHONE,REVENUE,UTILIZATION
public class OrganizationBLMapper : IOrganizationBLMapper
{
    private const int ExpectedColumnCount = 11;

    public OrganizationItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new OrganizationItem
        {
            Id = Guid.Parse(row[0]),
            Name = row[1],
            Address = NullIfEmpty(row[2]),
            City = NullIfEmpty(row[3]),
            State = NullIfEmpty(row[4]),
            Zip = NullIfEmpty(row[5]),
            Lat = ParseDecimal(row[6]),
            Lon = ParseDecimal(row[7]),
            Phone = NullIfEmpty(row[8]),
            Revenue = ParseDecimal(row[9]),
            Utilization = ParseInt(row[10]),
        };
    }

    public OrganizationItem ToItem(OrganizationRequest request) => new()
    {
        Name = request.Name,
        Address = request.Address,
        City = request.City,
        State = request.State,
        Zip = request.Zip,
        Phone = request.Phone,
        Lat = request.Lat,
        Lon = request.Lon,
        Revenue = request.Revenue,
        Utilization = request.Utilization
    };

    public OrganizationResponse ToResponse(OrganizationItem item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        Address = item.Address,
        City = item.City,
        State = item.State,
        Zip = item.Zip,
        Phone = item.Phone,
        Lat = item.Lat,
        Lon = item.Lon,
        Revenue = item.Revenue,
        Utilization = item.Utilization
    };

    private static decimal? ParseDecimal(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : decimal.Parse(value, CultureInfo.InvariantCulture);

    private static int? ParseInt(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : int.Parse(value, CultureInfo.InvariantCulture);

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
