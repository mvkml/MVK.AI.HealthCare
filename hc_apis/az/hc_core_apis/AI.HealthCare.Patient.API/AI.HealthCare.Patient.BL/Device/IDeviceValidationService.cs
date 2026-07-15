using AI.HealthCare.Patient.Models.Device;

namespace AI.HealthCare.Patient.BL;

public interface IDeviceValidationService
{
    DevicesModel Validate(DevicesModel devicesModel);
}
