using AI.HealthCare.Patient.Models.Device;

namespace AI.HealthCare.Patient.BL;

public class DeviceValidationService : IDeviceValidationService
{
    public DevicesModel Validate(DevicesModel devicesModel)
    {
        var request = devicesModel.DeviceRequest;

        if (request.PatientId == Guid.Empty)
        {
            devicesModel.IsNotValid = true;
            devicesModel.Message = "PatientId is required.";
            return devicesModel;
        }

        if (request.EncounterId == Guid.Empty)
        {
            devicesModel.IsNotValid = true;
            devicesModel.Message = "EncounterId is required.";
            return devicesModel;
        }

        devicesModel.IsNotValid = false;
        devicesModel.Message = "Validation passed.";
        return devicesModel;
    }
}
