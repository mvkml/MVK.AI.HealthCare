using AI.HealthCare.Patient.Models.Device;
using EfDevice = AI.HealthCare.Patient.EF.Entities.Device;

namespace AI.HealthCare.Patient.Repositories;

public class DeviceMapper : IDeviceMapper
{
    public DeviceItem ToModel(EfDevice entity) => new()
    {
        Id = entity.Id,
        Start = entity.Start,
        Stop = entity.Stop,
        PatientId = entity.PatientId,
        EncounterId = entity.EncounterId,
        Code = entity.Code,
        Description = entity.Description,
        Udi = entity.Udi
    };

    public EfDevice ToEntity(DeviceItem item) => new()
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
}
