using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.Immunization;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class ImmunizationRepository : IImmunizationRepository
{
    private readonly PatientDbContext _context;
    private readonly IImmunizationMapper _mapper;

    public ImmunizationRepository(PatientDbContext context, IImmunizationMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ImmunizationItem?> GetById(long id)
    {
        var entity = await _context.Immunizations.FirstOrDefaultAsync(i => i.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<ImmunizationItem>> GetAll()
    {
        var entities = await _context.Immunizations.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<List<ImmunizationItem>> GetByPatientId(Guid patientId)
    {
        var entities = await _context.Immunizations.Where(i => i.PatientId == patientId).ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<ImmunizationItem> Create(ImmunizationItem immunizationItem)
    {
        var entity = _mapper.ToEntity(immunizationItem);
        _context.Immunizations.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task CreateBatch(List<ImmunizationItem> immunizationItems)
    {
        var entities = immunizationItems.Select(_mapper.ToEntity);
        _context.Immunizations.AddRange(entities);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task UpsertBatch(List<ImmunizationItem> immunizationItems)
    {
        var patientIds = immunizationItems.Select(i => i.PatientId).ToHashSet();

        var existingEntities = await _context.Immunizations
            .Where(i => patientIds.Contains(i.PatientId))
            .ToListAsync();
        var existingByKey = existingEntities.ToLookup(e => (e.PatientId, e.EncounterId, e.Code));

        foreach (var item in immunizationItems)
        {
            var match = existingByKey[(item.PatientId, item.EncounterId, item.Code)].FirstOrDefault();
            if (match is not null)
            {
                match.Date = item.Date;
                match.Description = item.Description;
                match.BaseCost = item.BaseCost;
            }
            else
            {
                _context.Immunizations.Add(_mapper.ToEntity(item));
            }
        }

        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task<ImmunizationItem?> Update(ImmunizationItem immunizationItem)
    {
        var entity = await _context.Immunizations.FirstOrDefaultAsync(i => i.Id == immunizationItem.Id);
        if (entity is null) return null;

        entity.Date = immunizationItem.Date;
        entity.PatientId = immunizationItem.PatientId;
        entity.EncounterId = immunizationItem.EncounterId;
        entity.Code = immunizationItem.Code;
        entity.Description = immunizationItem.Description;
        entity.BaseCost = immunizationItem.BaseCost;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(long id)
    {
        var entity = await _context.Immunizations.FirstOrDefaultAsync(i => i.Id == id);
        if (entity is null) return false;

        _context.Immunizations.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
