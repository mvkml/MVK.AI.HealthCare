using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.Device;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly PatientDbContext _context;
    private readonly IDeviceMapper _mapper;

    public DeviceRepository(PatientDbContext context, IDeviceMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DeviceItem?> GetById(long id)
    {
        var entity = await _context.Devices.FirstOrDefaultAsync(d => d.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<DeviceItem>> GetAll()
    {
        var entities = await _context.Devices.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<List<DeviceItem>> GetByPatientId(Guid patientId)
    {
        var entities = await _context.Devices.Where(d => d.PatientId == patientId).ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<DeviceItem> Create(DeviceItem deviceItem)
    {
        var entity = _mapper.ToEntity(deviceItem);
        _context.Devices.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<DeviceItem?> Update(DeviceItem deviceItem)
    {
        var entity = await _context.Devices.FirstOrDefaultAsync(d => d.Id == deviceItem.Id);
        if (entity is null) return null;

        entity.Start = deviceItem.Start;
        entity.Stop = deviceItem.Stop;
        entity.PatientId = deviceItem.PatientId;
        entity.EncounterId = deviceItem.EncounterId;
        entity.Code = deviceItem.Code;
        entity.Description = deviceItem.Description;
        entity.Udi = deviceItem.Udi;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(long id)
    {
        var entity = await _context.Devices.FirstOrDefaultAsync(d => d.Id == id);
        if (entity is null) return false;

        _context.Devices.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
