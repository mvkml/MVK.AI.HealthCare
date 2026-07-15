using AI.HealthCare.Patient.Models.Device;

namespace AI.HealthCare.Patient.Repositories;

public interface IDeviceRepository
{
    Task<DeviceItem?> GetById(long id);
    Task<List<DeviceItem>> GetAll();
    Task<List<DeviceItem>> GetByPatientId(Guid patientId);
    Task<DeviceItem> Create(DeviceItem deviceItem);
    Task<DeviceItem?> Update(DeviceItem deviceItem);
    Task<bool> Delete(long id);
}
