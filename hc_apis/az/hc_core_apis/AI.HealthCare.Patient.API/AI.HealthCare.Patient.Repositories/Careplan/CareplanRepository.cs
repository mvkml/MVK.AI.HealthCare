using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.Careplan;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class CareplanRepository : ICareplanRepository
{
    private readonly PatientDbContext _context;
    private readonly ICareplanMapper _mapper;

    public CareplanRepository(PatientDbContext context, ICareplanMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CareplanItem?> GetById(Guid id)
    {
        var entity = await _context.Careplans.FirstOrDefaultAsync(c => c.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<CareplanItem>> GetAll()
    {
        var entities = await _context.Careplans.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<List<CareplanItem>> GetByPatientId(Guid patientId)
    {
        var entities = await _context.Careplans.Where(c => c.PatientId == patientId).ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<CareplanItem> Create(CareplanItem careplanItem)
    {
        var entity = _mapper.ToEntity(careplanItem);
        _context.Careplans.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task CreateBatch(List<CareplanItem> careplanItems)
    {
        var entities = careplanItems.Select(_mapper.ToEntity);
        _context.Careplans.AddRange(entities);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task UpsertBatch(List<CareplanItem> careplanItems)
    {
        var ids = careplanItems.Select(c => c.Id).ToHashSet();
        var existingIds = (await _context.Careplans
            .Where(c => ids.Contains(c.Id))
            .Select(c => c.Id)
            .ToListAsync())
            .ToHashSet();

        foreach (var item in careplanItems)
        {
            var entity = _mapper.ToEntity(item);
            if (existingIds.Contains(item.Id))
                _context.Careplans.Update(entity);
            else
                _context.Careplans.Add(entity);
        }

        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task<CareplanItem?> Update(CareplanItem careplanItem)
    {
        var entity = await _context.Careplans.FirstOrDefaultAsync(c => c.Id == careplanItem.Id);
        if (entity is null) return null;

        entity.Start = careplanItem.Start;
        entity.Stop = careplanItem.Stop;
        entity.PatientId = careplanItem.PatientId;
        entity.EncounterId = careplanItem.EncounterId;
        entity.Code = careplanItem.Code;
        entity.Description = careplanItem.Description;
        entity.ReasonCode = careplanItem.ReasonCode;
        entity.ReasonDescription = careplanItem.ReasonDescription;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await _context.Careplans.FirstOrDefaultAsync(c => c.Id == id);
        if (entity is null) return false;

        _context.Careplans.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
