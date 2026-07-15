using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.Encounter;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class EncounterRepository : IEncounterRepository
{
    private readonly PatientDbContext _context;
    private readonly IEncounterMapper _mapper;

    public EncounterRepository(PatientDbContext context, IEncounterMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<EncounterItem?> GetById(Guid id)
    {
        var entity = await _context.Encounters.FirstOrDefaultAsync(e => e.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<EncounterItem>> GetAll()
    {
        var entities = await _context.Encounters.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<List<EncounterItem>> GetByPatientId(Guid patientId)
    {
        var entities = await _context.Encounters.Where(e => e.PatientId == patientId).ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<EncounterItem> Create(EncounterItem encounterItem)
    {
        var entity = _mapper.ToEntity(encounterItem);
        _context.Encounters.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task CreateBatch(List<EncounterItem> encounterItems)
    {
        var entities = encounterItems.Select(_mapper.ToEntity);
        _context.Encounters.AddRange(entities);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task UpsertBatch(List<EncounterItem> encounterItems)
    {
        var ids = encounterItems.Select(e => e.Id).ToHashSet();
        var existingIds = (await _context.Encounters
            .Where(e => ids.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync())
            .ToHashSet();

        foreach (var item in encounterItems)
        {
            var entity = _mapper.ToEntity(item);
            if (existingIds.Contains(item.Id))
                _context.Encounters.Update(entity);
            else
                _context.Encounters.Add(entity);
        }

        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task<EncounterItem?> Update(EncounterItem encounterItem)
    {
        var entity = await _context.Encounters.FirstOrDefaultAsync(e => e.Id == encounterItem.Id);
        if (entity is null) return null;

        entity.Start = encounterItem.Start;
        entity.Stop = encounterItem.Stop;
        entity.PatientId = encounterItem.PatientId;
        entity.OrganizationId = encounterItem.OrganizationId;
        entity.ProviderId = encounterItem.ProviderId;
        entity.PayerId = encounterItem.PayerId;
        entity.EncounterClass = encounterItem.EncounterClass;
        entity.Code = encounterItem.Code;
        entity.Description = encounterItem.Description;
        entity.BaseEncounterCost = encounterItem.BaseEncounterCost;
        entity.TotalClaimCost = encounterItem.TotalClaimCost;
        entity.PayerCoverage = encounterItem.PayerCoverage;
        entity.ReasonCode = encounterItem.ReasonCode;
        entity.ReasonDescription = encounterItem.ReasonDescription;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await _context.Encounters.FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null) return false;

        _context.Encounters.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
