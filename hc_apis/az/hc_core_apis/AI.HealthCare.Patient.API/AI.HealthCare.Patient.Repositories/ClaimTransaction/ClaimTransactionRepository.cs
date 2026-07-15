using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.ClaimTransaction;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class ClaimTransactionRepository : IClaimTransactionRepository
{
    private readonly PatientDbContext _context;
    private readonly IClaimTransactionMapper _mapper;

    public ClaimTransactionRepository(PatientDbContext context, IClaimTransactionMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ClaimTransactionItem?> GetById(Guid id)
    {
        var entity = await _context.ClaimTransactions.FirstOrDefaultAsync(c => c.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<ClaimTransactionItem>> GetAll()
    {
        var entities = await _context.ClaimTransactions.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<List<ClaimTransactionItem>> GetByClaimId(Guid claimId)
    {
        var entities = await _context.ClaimTransactions.Where(c => c.ClaimId == claimId).ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<ClaimTransactionItem> Create(ClaimTransactionItem claimTransactionItem)
    {
        var entity = _mapper.ToEntity(claimTransactionItem);
        _context.ClaimTransactions.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<ClaimTransactionItem?> Update(ClaimTransactionItem claimTransactionItem)
    {
        var entity = await _context.ClaimTransactions.FirstOrDefaultAsync(c => c.Id == claimTransactionItem.Id);
        if (entity is null) return null;

        entity.ClaimId = claimTransactionItem.ClaimId;
        entity.ChargeId = claimTransactionItem.ChargeId;
        entity.PatientId = claimTransactionItem.PatientId;
        entity.Type = claimTransactionItem.Type;
        entity.Amount = claimTransactionItem.Amount;
        entity.Method = claimTransactionItem.Method;
        entity.FromDate = claimTransactionItem.FromDate;
        entity.ToDate = claimTransactionItem.ToDate;
        entity.PlaceOfServiceId = claimTransactionItem.PlaceOfServiceId;
        entity.ProcedureCode = claimTransactionItem.ProcedureCode;
        entity.Modifier1 = claimTransactionItem.Modifier1;
        entity.Modifier2 = claimTransactionItem.Modifier2;
        entity.DiagnosisRef1 = claimTransactionItem.DiagnosisRef1;
        entity.DiagnosisRef2 = claimTransactionItem.DiagnosisRef2;
        entity.DiagnosisRef3 = claimTransactionItem.DiagnosisRef3;
        entity.DiagnosisRef4 = claimTransactionItem.DiagnosisRef4;
        entity.Units = claimTransactionItem.Units;
        entity.DepartmentId = claimTransactionItem.DepartmentId;
        entity.Notes = claimTransactionItem.Notes;
        entity.UnitAmount = claimTransactionItem.UnitAmount;
        entity.TransferOutId = claimTransactionItem.TransferOutId;
        entity.TransferType = claimTransactionItem.TransferType;
        entity.Payments = claimTransactionItem.Payments;
        entity.Adjustments = claimTransactionItem.Adjustments;
        entity.Transfers = claimTransactionItem.Transfers;
        entity.Outstanding = claimTransactionItem.Outstanding;
        entity.AppointmentId = claimTransactionItem.AppointmentId;
        entity.LineNote = claimTransactionItem.LineNote;
        entity.PatientInsuranceId = claimTransactionItem.PatientInsuranceId;
        entity.FeeScheduleId = claimTransactionItem.FeeScheduleId;
        entity.ProviderId = claimTransactionItem.ProviderId;
        entity.SupervisingProviderId = claimTransactionItem.SupervisingProviderId;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await _context.ClaimTransactions.FirstOrDefaultAsync(c => c.Id == id);
        if (entity is null) return false;

        _context.ClaimTransactions.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
