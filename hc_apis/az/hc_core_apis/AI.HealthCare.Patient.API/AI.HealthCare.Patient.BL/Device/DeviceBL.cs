using AI.HealthCare.Patient.Models.Device;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class DeviceBL : IDeviceBL
{
    private readonly IDeviceRepository _deviceRepository;

    public DeviceBL(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<DevicesModel> Create(DevicesModel devicesModel)
    {
        devicesModel.DeviceItem = ToItem(devicesModel.DeviceRequest);

        var savedItem = await _deviceRepository.Create(devicesModel.DeviceItem);
        devicesModel.DeviceItem = savedItem;

        devicesModel.DeviceResponse = ToResponse(savedItem);
        devicesModel.IsNotValid = false;
        devicesModel.Message = "Device created successfully.";

        return devicesModel;
    }

    public async Task<DevicesModel> GetById(DevicesModel devicesModel)
    {
        var item = await _deviceRepository.GetById(devicesModel.DeviceItem.Id);
        if (item is null)
        {
            devicesModel.IsNotValid = true;
            devicesModel.Message = "Device not found.";
            return devicesModel;
        }

        devicesModel.DeviceItem = item;
        devicesModel.DeviceResponse = ToResponse(item);
        devicesModel.IsNotValid = false;
        devicesModel.Message = "Device retrieved successfully.";
        return devicesModel;
    }

    public async Task<DevicesModel> GetAll(DevicesModel devicesModel)
    {
        var items = await _deviceRepository.GetAll();
        devicesModel.DeviceItems = items;
        devicesModel.IsNotValid = false;
        devicesModel.Message = $"{items.Count} device(s) retrieved.";
        return devicesModel;
    }

    public async Task<DevicesModel> GetByPatientId(Guid patientId)
    {
        var items = await _deviceRepository.GetByPatientId(patientId);
        return new DevicesModel
        {
            DeviceItems = items,
            IsNotValid = false,
            Message = $"{items.Count} device(s) retrieved for patient."
        };
    }

    public async Task<DevicesModel> Update(DevicesModel devicesModel)
    {
        var existing = await _deviceRepository.GetById(devicesModel.DeviceItem.Id);
        if (existing is null)
        {
            devicesModel.IsNotValid = true;
            devicesModel.Message = "Device not found.";
            return devicesModel;
        }

        var updatedItem = ToItem(devicesModel.DeviceRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _deviceRepository.Update(updatedItem);
        devicesModel.DeviceItem = savedItem!;
        devicesModel.DeviceResponse = ToResponse(savedItem!);
        devicesModel.IsNotValid = false;
        devicesModel.Message = "Device updated successfully.";
        return devicesModel;
    }

    public async Task<DevicesModel> Delete(DevicesModel devicesModel)
    {
        var deleted = await _deviceRepository.Delete(devicesModel.DeviceItem.Id);
        if (!deleted)
        {
            devicesModel.IsNotValid = true;
            devicesModel.Message = "Device not found.";
            return devicesModel;
        }

        devicesModel.IsNotValid = false;
        devicesModel.Message = "Device deleted successfully.";
        return devicesModel;
    }

    private static DeviceItem ToItem(DeviceRequest request) => new()
    {
        Start = request.Start,
        Stop = request.Stop,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        Code = request.Code,
        Description = request.Description,
        Udi = request.Udi
    };

    private static DeviceResponse ToResponse(DeviceItem item) => new()
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
