using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.Procedure;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class ProcedureRepository : IProcedureRepository
{
    private readonly PatientDbContext _context;
    private readonly IProcedureMapper _mapper;

    public ProcedureRepository(PatientDbContext context, IProcedureMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProcedureItem?> GetById(long id)
    {
        var entity = await _context.Procedures.FirstOrDefaultAsync(p => p.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<ProcedureItem>> GetAll()
    {
        var entities = await _context.Procedures.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<List<ProcedureItem>> GetByPatientId(Guid patientId)
    {
        var entities = await _context.Procedures.Where(p => p.PatientId == patientId).ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<ProcedureItem> Create(ProcedureItem procedureItem)
    {
        var entity = _mapper.ToEntity(procedureItem);
        _context.Procedures.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task CreateBatch(List<ProcedureItem> procedureItems)
    {
        var entities = procedureItems.Select(_mapper.ToEntity);
        _context.Procedures.AddRange(entities);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task UpsertBatch(List<ProcedureItem> procedureItems)
    {
        var patientIds = procedureItems.Select(p => p.PatientId).ToHashSet();

        var existingEntities = await _context.Procedures
            .Where(p => patientIds.Contains(p.PatientId))
            .ToListAsync();
        var existingByKey = existingEntities.ToLookup(e => (e.PatientId, e.EncounterId, e.Code));

        foreach (var item in procedureItems)
        {
            var match = existingByKey[(item.PatientId, item.EncounterId, item.Code)].FirstOrDefault();
            if (match is not null)
            {
                match.Start = item.Start;
                match.Stop = item.Stop;
                match.System = item.System;
                match.Description = item.Description;
                match.BaseCost = item.BaseCost;
                match.ReasonCode = item.ReasonCode;
                match.ReasonDescription = item.ReasonDescription;
            }
            else
            {
                _context.Procedures.Add(_mapper.ToEntity(item));
            }
        }

        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task<ProcedureItem?> Update(ProcedureItem procedureItem)
    {
        var entity = await _context.Procedures.FirstOrDefaultAsync(p => p.Id == procedureItem.Id);
        if (entity is null) return null;

        entity.Start = procedureItem.Start;
        entity.Stop = procedureItem.Stop;
        entity.PatientId = procedureItem.PatientId;
        entity.EncounterId = procedureItem.EncounterId;
        entity.System = procedureItem.System;
        entity.Code = procedureItem.Code;
        entity.Description = procedureItem.Description;
        entity.BaseCost = procedureItem.BaseCost;
        entity.ReasonCode = procedureItem.ReasonCode;
        entity.ReasonDescription = procedureItem.ReasonDescription;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(long id)
    {
        var entity = await _context.Procedures.FirstOrDefaultAsync(p => p.Id == id);
        if (entity is null) return false;

        _context.Procedures.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
