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
