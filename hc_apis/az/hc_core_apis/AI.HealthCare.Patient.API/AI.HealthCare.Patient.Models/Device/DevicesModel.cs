namespace AI.HealthCare.Patient.Models.Device;

public class DevicesModel : BaseModel
{
    public DeviceRequest DeviceRequest { get; set; } = new();
    public DeviceItem DeviceItem { get; set; } = new();
    public List<DeviceItem> DeviceItems { get; set; } = new();
    public DeviceResponse DeviceResponse { get; set; } = new();
}
