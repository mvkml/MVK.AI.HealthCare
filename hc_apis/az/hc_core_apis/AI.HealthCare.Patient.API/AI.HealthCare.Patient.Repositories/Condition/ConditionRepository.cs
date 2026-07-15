using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.Condition;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class ConditionRepository : IConditionRepository
{
    private readonly PatientDbContext _context;
    private readonly IConditionMapper _mapper;

    public ConditionRepository(PatientDbContext context, IConditionMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ConditionItem?> GetById(long id)
    {
        var entity = await _context.Conditions.FirstOrDefaultAsync(c => c.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<ConditionItem>> GetAll()
    {
        var entities = await _context.Conditions.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<List<ConditionItem>> GetByPatientId(Guid patientId)
    {
        var entities = await _context.Conditions.Where(c => c.PatientId == patientId).ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<ConditionItem> Create(ConditionItem conditionItem)
    {
        var entity = _mapper.ToEntity(conditionItem);
        _context.Conditions.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task CreateBatch(List<ConditionItem> conditionItems)
    {
        var entities = conditionItems.Select(_mapper.ToEntity);
        _context.Conditions.AddRange(entities);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task<ConditionItem?> Update(ConditionItem conditionItem)
    {
        var entity = await _context.Conditions.FirstOrDefaultAsync(c => c.Id == conditionItem.Id);
        if (entity is null) return null;

        entity.Start = conditionItem.Start;
        entity.Stop = conditionItem.Stop;
        entity.PatientId = conditionItem.PatientId;
        entity.EncounterId = conditionItem.EncounterId;
        entity.System = conditionItem.System;
        entity.Code = conditionItem.Code;
        entity.Description = conditionItem.Description;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(long id)
    {
        var entity = await _context.Conditions.FirstOrDefaultAsync(c => c.Id == id);
        if (entity is null) return false;

        _context.Conditions.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
