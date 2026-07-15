using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.Allergy;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class AllergyRepository : IAllergyRepository
{
    private readonly PatientDbContext _context;
    private readonly IAllergyMapper _mapper;

    public AllergyRepository(PatientDbContext context, IAllergyMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AllergyItem?> GetById(long id)
    {
        var entity = await _context.Allergies.FirstOrDefaultAsync(a => a.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<AllergyItem>> GetAll()
    {
        var entities = await _context.Allergies.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<List<AllergyItem>> GetByPatientId(Guid patientId)
    {
        var entities = await _context.Allergies.Where(a => a.PatientId == patientId).ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<AllergyItem> Create(AllergyItem allergyItem)
    {
        var entity = _mapper.ToEntity(allergyItem);
        _context.Allergies.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task CreateBatch(List<AllergyItem> allergyItems)
    {
        var entities = allergyItems.Select(_mapper.ToEntity);
        _context.Allergies.AddRange(entities);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task UpsertBatch(List<AllergyItem> allergyItems)
    {
        var patientIds = allergyItems.Select(a => a.PatientId).ToHashSet();

        var existingEntities = await _context.Allergies
            .Where(a => patientIds.Contains(a.PatientId))
            .ToListAsync();
        var existingByKey = existingEntities.ToLookup(e => (e.PatientId, e.EncounterId, e.Code));

        foreach (var item in allergyItems)
        {
            var match = existingByKey[(item.PatientId, item.EncounterId, item.Code)].FirstOrDefault();
            if (match is not null)
            {
                match.Start = item.Start;
                match.Stop = item.Stop;
                match.System = item.System;
                match.Description = item.Description;
                match.Type = item.Type;
                match.Category = item.Category;
                match.Reaction1 = item.Reaction1;
                match.Description1 = item.Description1;
                match.Severity1 = item.Severity1;
                match.Reaction2 = item.Reaction2;
                match.Description2 = item.Description2;
                match.Severity2 = item.Severity2;
            }
            else
            {
                _context.Allergies.Add(_mapper.ToEntity(item));
            }
        }

        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task<AllergyItem?> Update(AllergyItem allergyItem)
    {
        var entity = await _context.Allergies.FirstOrDefaultAsync(a => a.Id == allergyItem.Id);
        if (entity is null) return null;

        entity.Start = allergyItem.Start;
        entity.Stop = allergyItem.Stop;
        entity.PatientId = allergyItem.PatientId;
        entity.EncounterId = allergyItem.EncounterId;
        entity.Code = allergyItem.Code;
        entity.System = allergyItem.System;
        entity.Description = allergyItem.Description;
        entity.Type = allergyItem.Type;
        entity.Category = allergyItem.Category;
        entity.Reaction1 = allergyItem.Reaction1;
        entity.Description1 = allergyItem.Description1;
        entity.Severity1 = allergyItem.Severity1;
        entity.Reaction2 = allergyItem.Reaction2;
        entity.Description2 = allergyItem.Description2;
        entity.Severity2 = allergyItem.Severity2;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(long id)
    {
        var entity = await _context.Allergies.FirstOrDefaultAsync(a => a.Id == id);
        if (entity is null) return false;

        _context.Allergies.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
