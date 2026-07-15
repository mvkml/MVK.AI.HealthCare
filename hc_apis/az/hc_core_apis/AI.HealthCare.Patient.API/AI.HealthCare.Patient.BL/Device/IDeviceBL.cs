using AI.HealthCare.Patient.Models.Device;
using AI.HealthCare.Patient.Models.Shared;

namespace AI.HealthCare.Patient.BL;

public interface IDeviceBL
{
    Task<DevicesModel> Create(DevicesModel devicesModel);
    Task<DevicesModel> GetById(DevicesModel devicesModel);
    Task<DevicesModel> GetAll(DevicesModel devicesModel);
    Task<DevicesModel> GetByPatientId(Guid patientId);
    Task<DevicesModel> Update(DevicesModel devicesModel);
    Task<DevicesModel> Delete(DevicesModel devicesModel);
    Task<ImportResult> Import(Stream csvStream);
    Task<ImportResult> ImportUpsert(Stream csvStream);
}
