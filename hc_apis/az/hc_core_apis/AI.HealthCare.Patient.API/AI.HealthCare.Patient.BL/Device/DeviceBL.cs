using AI.HealthCare.Patient.Models.Device;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class DeviceBL : IDeviceBL
{
    private const int ImportBatchSize = 500;

    private readonly IDeviceRepository _deviceRepository;
    private readonly IDeviceBLMapper _mapper;

    public DeviceBL(IDeviceRepository deviceRepository, IDeviceBLMapper mapper)
    {
        _deviceRepository = deviceRepository;
        _mapper = mapper;
    }

    public async Task<DevicesModel> Create(DevicesModel devicesModel)
    {
        devicesModel.DeviceItem = _mapper.ToItem(devicesModel.DeviceRequest);

        var savedItem = await _deviceRepository.Create(devicesModel.DeviceItem);
        devicesModel.DeviceItem = savedItem;

        devicesModel.DeviceResponse = _mapper.ToResponse(savedItem);
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
        devicesModel.DeviceResponse = _mapper.ToResponse(item);
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

        var updatedItem = _mapper.ToItem(devicesModel.DeviceRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _deviceRepository.Update(updatedItem);
        devicesModel.DeviceItem = savedItem!;
        devicesModel.DeviceResponse = _mapper.ToResponse(savedItem!);
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

    public async Task<ImportResult> Import(Stream csvStream) =>
        await RunImport(csvStream, FlushBatch);

    public async Task<ImportResult> ImportUpsert(Stream csvStream) =>
        await RunImport(csvStream, FlushUpsertBatch);

    private async Task<ImportResult> RunImport(Stream csvStream, Func<List<DeviceItem>, ImportResult, Task> flush)
    {
        var result = new ImportResult();
        var batch = new List<DeviceItem>();

        using var reader = new StreamReader(csvStream);
        await reader.ReadLineAsync(); // skip header row

        var rowNumber = 1;
        while (!reader.EndOfStream)
        {
            rowNumber++;
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            result.TotalRows++;

            try
            {
                batch.Add(_mapper.ToModel(line.Split(',')));
            }
            catch (Exception ex)
            {
                result.FailedCount++;
                result.Errors.Add(new ImportRowError { RowNumber = rowNumber, ErrorMessage = ex.Message });
                continue;
            }

            if (batch.Count >= ImportBatchSize)
                await flush(batch, result);
        }

        if (batch.Count > 0)
            await flush(batch, result);

        return result;
    }

    private async Task FlushBatch(List<DeviceItem> batch, ImportResult result)
    {
        try
        {
            await _deviceRepository.CreateBatch(batch);
            result.InsertedCount += batch.Count;
        }
        catch (Exception ex)
        {
            result.FailedCount += batch.Count;
            result.Errors.Add(new ImportRowError { RowNumber = -1, ErrorMessage = $"Batch insert failed: {ex.Message}" });
        }
        finally
        {
            batch.Clear();
        }
    }

    private async Task FlushUpsertBatch(List<DeviceItem> batch, ImportResult result)
    {
        try
        {
            await _deviceRepository.UpsertBatch(batch);
            result.InsertedCount += batch.Count;
        }
        catch (Exception ex)
        {
            result.FailedCount += batch.Count;
            result.Errors.Add(new ImportRowError { RowNumber = -1, ErrorMessage = $"Batch upsert failed: {ex.Message}" });
        }
        finally
        {
            batch.Clear();
        }
    }
}
