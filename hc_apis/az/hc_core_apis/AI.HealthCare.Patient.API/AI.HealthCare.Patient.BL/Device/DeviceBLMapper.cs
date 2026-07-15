using System.Globalization;
using AI.HealthCare.Patient.Models.Device;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's devices.csv:
// START,STOP,PATIENT,ENCOUNTER,CODE,DESCRIPTION,UDI
public class DeviceBLMapper : IDeviceBLMapper
{
    private const int ExpectedColumnCount = 7;

    public DeviceItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new DeviceItem
        {
            Start = ParseDate(row[0]),
            Stop = ParseDate(row[1]),
            PatientId = Guid.Parse(row[2]),
            EncounterId = Guid.Parse(row[3]),
            Code = NullIfEmpty(row[4]),
            Description = NullIfEmpty(row[5]),
            Udi = NullIfEmpty(row[6]),
        };
    }

    public DeviceItem ToItem(DeviceRequest request) => new()
    {
        Start = request.Start,
        Stop = request.Stop,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        Code = request.Code,
        Description = request.Description,
        Udi = request.Udi
    };

    public DeviceResponse ToResponse(DeviceItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        Code = item.Code,
        Description = item.Description,
        Udi = item.Udi
    };

    private static DateTime? ParseDate(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
