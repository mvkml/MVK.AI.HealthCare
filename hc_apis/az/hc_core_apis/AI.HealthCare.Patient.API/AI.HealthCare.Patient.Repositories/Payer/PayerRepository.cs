using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.Payer;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class PayerRepository : IPayerRepository
{
    private readonly PatientDbContext _context;
    private readonly IPayerMapper _mapper;

    public PayerRepository(PatientDbContext context, IPayerMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PayerItem?> GetById(Guid id)
    {
        var entity = await _context.Payers.FirstOrDefaultAsync(p => p.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<PayerItem>> GetAll()
    {
        var entities = await _context.Payers.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<PayerItem> Create(PayerItem payerItem)
    {
        var entity = _mapper.ToEntity(payerItem);
        _context.Payers.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task CreateBatch(List<PayerItem> payerItems)
    {
        var entities = payerItems.Select(_mapper.ToEntity);
        _context.Payers.AddRange(entities);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task<PayerItem?> Update(PayerItem payerItem)
    {
        var entity = await _context.Payers.FirstOrDefaultAsync(p => p.Id == payerItem.Id);
        if (entity is null) return null;

        entity.Name = payerItem.Name;
        entity.Ownership = payerItem.Ownership;
        entity.Address = payerItem.Address;
        entity.City = payerItem.City;
        entity.StateHeadquartered = payerItem.StateHeadquartered;
        entity.Zip = payerItem.Zip;
        entity.Phone = payerItem.Phone;
        entity.AmountCovered = payerItem.AmountCovered;
        entity.AmountUncovered = payerItem.AmountUncovered;
        entity.Revenue = payerItem.Revenue;
        entity.CoveredEncounters = payerItem.CoveredEncounters;
        entity.UncoveredEncounters = payerItem.UncoveredEncounters;
        entity.CoveredMedications = payerItem.CoveredMedications;
        entity.UncoveredMedications = payerItem.UncoveredMedications;
        entity.CoveredProcedures = payerItem.CoveredProcedures;
        entity.UncoveredProcedures = payerItem.UncoveredProcedures;
        entity.CoveredImmunizations = payerItem.CoveredImmunizations;
        entity.UncoveredImmunizations = payerItem.UncoveredImmunizations;
        entity.UniqueCustomers = payerItem.UniqueCustomers;
        entity.QolsAvg = payerItem.QolsAvg;
        entity.MemberMonths = payerItem.MemberMonths;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await _context.Payers.FirstOrDefaultAsync(p => p.Id == id);
        if (entity is null) return false;

        _context.Payers.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
