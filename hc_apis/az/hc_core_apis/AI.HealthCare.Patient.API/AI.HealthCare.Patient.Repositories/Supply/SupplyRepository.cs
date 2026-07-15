using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.Supply;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class SupplyRepository : ISupplyRepository
{
    private readonly PatientDbContext _context;
    private readonly ISupplyMapper _mapper;

    public SupplyRepository(PatientDbContext context, ISupplyMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<SupplyItem?> GetById(long id)
    {
        var entity = await _context.Supplies.FirstOrDefaultAsync(s => s.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<SupplyItem>> GetAll()
    {
        var entities = await _context.Supplies.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<List<SupplyItem>> GetByPatientId(Guid patientId)
    {
        var entities = await _context.Supplies.Where(s => s.PatientId == patientId).ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<SupplyItem> Create(SupplyItem supplyItem)
    {
        var entity = _mapper.ToEntity(supplyItem);
        _context.Supplies.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task CreateBatch(List<SupplyItem> supplyItems)
    {
        var entities = supplyItems.Select(_mapper.ToEntity);
        _context.Supplies.AddRange(entities);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task<SupplyItem?> Update(SupplyItem supplyItem)
    {
        var entity = await _context.Supplies.FirstOrDefaultAsync(s => s.Id == supplyItem.Id);
        if (entity is null) return null;

        entity.Date = supplyItem.Date;
        entity.PatientId = supplyItem.PatientId;
        entity.EncounterId = supplyItem.EncounterId;
        entity.Code = supplyItem.Code;
        entity.Description = supplyItem.Description;
        entity.Quantity = supplyItem.Quantity;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(long id)
    {
        var entity = await _context.Supplies.FirstOrDefaultAsync(s => s.Id == id);
        if (entity is null) return false;

        _context.Supplies.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
