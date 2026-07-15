using AI.HealthCare.Patient.Models.Device;
using EfDevice = AI.HealthCare.Patient.EF.Entities.Device;

namespace AI.HealthCare.Patient.Repositories;

public interface IDeviceMapper
{
    DeviceItem ToModel(EfDevice entity);
    EfDevice ToEntity(DeviceItem item);
}
