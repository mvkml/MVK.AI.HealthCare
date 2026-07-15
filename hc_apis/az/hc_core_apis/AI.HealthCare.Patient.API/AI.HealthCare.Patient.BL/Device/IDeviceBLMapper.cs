using AI.HealthCare.Patient.Models.Device;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface IDeviceBLMapper : ICsvRowParser<DeviceItem>
{
    DeviceItem ToItem(DeviceRequest request);
    DeviceResponse ToResponse(DeviceItem item);
}
