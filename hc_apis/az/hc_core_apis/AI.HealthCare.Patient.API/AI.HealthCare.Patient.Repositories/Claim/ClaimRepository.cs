using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.Claim;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class ClaimRepository : IClaimRepository
{
    private readonly PatientDbContext _context;
    private readonly IClaimMapper _mapper;

    public ClaimRepository(PatientDbContext context, IClaimMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ClaimItem?> GetById(Guid id)
    {
        var entity = await _context.Claims.FirstOrDefaultAsync(c => c.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<ClaimItem>> GetAll()
    {
        var entities = await _context.Claims.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<List<ClaimItem>> GetByPatientId(Guid patientId)
    {
        var entities = await _context.Claims.Where(c => c.PatientId == patientId).ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<ClaimItem> Create(ClaimItem claimItem)
    {
        var entity = _mapper.ToEntity(claimItem);
        _context.Claims.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task CreateBatch(List<ClaimItem> claimItems)
    {
        var entities = claimItems.Select(_mapper.ToEntity);
        _context.Claims.AddRange(entities);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task UpsertBatch(List<ClaimItem> claimItems)
    {
        var ids = claimItems.Select(c => c.Id).ToHashSet();
        var existingIds = (await _context.Claims
            .Where(c => ids.Contains(c.Id))
            .Select(c => c.Id)
            .ToListAsync())
            .ToHashSet();

        foreach (var item in claimItems)
        {
            var entity = _mapper.ToEntity(item);
            if (existingIds.Contains(item.Id))
                _context.Claims.Update(entity);
            else
                _context.Claims.Add(entity);
        }

        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task<ClaimItem?> Update(ClaimItem claimItem)
    {
        var entity = await _context.Claims.FirstOrDefaultAsync(c => c.Id == claimItem.Id);
        if (entity is null) return null;

        entity.PatientId = claimItem.PatientId;
        entity.ProviderId = claimItem.ProviderId;
        entity.PrimaryPatientInsuranceId = claimItem.PrimaryPatientInsuranceId;
        entity.SecondaryPatientInsuranceId = claimItem.SecondaryPatientInsuranceId;
        entity.DepartmentId = claimItem.DepartmentId;
        entity.PatientDepartmentId = claimItem.PatientDepartmentId;
        entity.Diagnosis1 = claimItem.Diagnosis1;
        entity.Diagnosis2 = claimItem.Diagnosis2;
        entity.Diagnosis3 = claimItem.Diagnosis3;
        entity.Diagnosis4 = claimItem.Diagnosis4;
        entity.Diagnosis5 = claimItem.Diagnosis5;
        entity.Diagnosis6 = claimItem.Diagnosis6;
        entity.Diagnosis7 = claimItem.Diagnosis7;
        entity.Diagnosis8 = claimItem.Diagnosis8;
        entity.ReferringProviderId = claimItem.ReferringProviderId;
        entity.SupervisingProviderId = claimItem.SupervisingProviderId;
        entity.AppointmentId = claimItem.AppointmentId;
        entity.CurrentIllnessDate = claimItem.CurrentIllnessDate;
        entity.ServiceDate = claimItem.ServiceDate;
        entity.Status1 = claimItem.Status1;
        entity.Status2 = claimItem.Status2;
        entity.StatusP = claimItem.StatusP;
        entity.Outstanding1 = claimItem.Outstanding1;
        entity.Outstanding2 = claimItem.Outstanding2;
        entity.OutstandingP = claimItem.OutstandingP;
        entity.LastBilledDate1 = claimItem.LastBilledDate1;
        entity.LastBilledDate2 = claimItem.LastBilledDate2;
        entity.LastBilledDateP = claimItem.LastBilledDateP;
        entity.HealthcareClaimTypeId1 = claimItem.HealthcareClaimTypeId1;
        entity.HealthcareClaimTypeId2 = claimItem.HealthcareClaimTypeId2;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await _context.Claims.FirstOrDefaultAsync(c => c.Id == id);
        if (entity is null) return false;

        _context.Claims.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
