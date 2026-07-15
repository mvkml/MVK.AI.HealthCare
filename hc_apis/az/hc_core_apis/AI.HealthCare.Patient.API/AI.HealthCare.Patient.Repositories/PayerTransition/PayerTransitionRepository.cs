using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.PayerTransition;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class PayerTransitionRepository : IPayerTransitionRepository
{
    private readonly PatientDbContext _context;
    private readonly IPayerTransitionMapper _mapper;

    public PayerTransitionRepository(PatientDbContext context, IPayerTransitionMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PayerTransitionItem?> GetById(long id)
    {
        var entity = await _context.PayerTransitions.FirstOrDefaultAsync(pt => pt.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<PayerTransitionItem>> GetAll()
    {
        var entities = await _context.PayerTransitions.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<PayerTransitionItem> Create(PayerTransitionItem payerTransitionItem)
    {
        var entity = _mapper.ToEntity(payerTransitionItem);
        _context.PayerTransitions.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task CreateBatch(List<PayerTransitionItem> payerTransitionItems)
    {
        var entities = payerTransitionItems.Select(_mapper.ToEntity);
        _context.PayerTransitions.AddRange(entities);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task UpsertBatch(List<PayerTransitionItem> payerTransitionItems)
    {
        var patientIds = payerTransitionItems.Select(pt => pt.PatientId).ToHashSet();

        var existingEntities = await _context.PayerTransitions
            .Where(pt => patientIds.Contains(pt.PatientId))
            .ToListAsync();
        var existingByKey = existingEntities.ToLookup(e => (e.PatientId, e.MemberId));

        foreach (var item in payerTransitionItems)
        {
            var match = existingByKey[(item.PatientId, item.MemberId)].FirstOrDefault();
            if (match is not null)
            {
                match.StartDate = item.StartDate;
                match.EndDate = item.EndDate;
                match.PayerId = item.PayerId;
                match.SecondaryPayerId = item.SecondaryPayerId;
                match.PlanOwnership = item.PlanOwnership;
                match.OwnerName = item.OwnerName;
            }
            else
            {
                _context.PayerTransitions.Add(_mapper.ToEntity(item));
            }
        }

        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task<PayerTransitionItem?> Update(PayerTransitionItem payerTransitionItem)
    {
        var entity = await _context.PayerTransitions.FirstOrDefaultAsync(pt => pt.Id == payerTransitionItem.Id);
        if (entity is null) return null;

        entity.PatientId = payerTransitionItem.PatientId;
        entity.MemberId = payerTransitionItem.MemberId;
        entity.StartDate = payerTransitionItem.StartDate;
        entity.EndDate = payerTransitionItem.EndDate;
        entity.PayerId = payerTransitionItem.PayerId;
        entity.SecondaryPayerId = payerTransitionItem.SecondaryPayerId;
        entity.PlanOwnership = payerTransitionItem.PlanOwnership;
        entity.OwnerName = payerTransitionItem.OwnerName;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(long id)
    {
        var entity = await _context.PayerTransitions.FirstOrDefaultAsync(pt => pt.Id == id);
        if (entity is null) return false;

        _context.PayerTransitions.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
