using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.Medication;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class MedicationRepository : IMedicationRepository
{
    private readonly PatientDbContext _context;
    private readonly IMedicationMapper _mapper;

    public MedicationRepository(PatientDbContext context, IMedicationMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<MedicationItem?> GetById(long id)
    {
        var entity = await _context.Medications.FirstOrDefaultAsync(m => m.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<MedicationItem>> GetAll()
    {
        var entities = await _context.Medications.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<List<MedicationItem>> GetByPatientId(Guid patientId)
    {
        var entities = await _context.Medications.Where(m => m.PatientId == patientId).ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<MedicationItem> Create(MedicationItem medicationItem)
    {
        var entity = _mapper.ToEntity(medicationItem);
        _context.Medications.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task CreateBatch(List<MedicationItem> medicationItems)
    {
        var entities = medicationItems.Select(_mapper.ToEntity);
        _context.Medications.AddRange(entities);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task<MedicationItem?> Update(MedicationItem medicationItem)
    {
        var entity = await _context.Medications.FirstOrDefaultAsync(m => m.Id == medicationItem.Id);
        if (entity is null) return null;

        entity.Start = medicationItem.Start;
        entity.Stop = medicationItem.Stop;
        entity.PatientId = medicationItem.PatientId;
        entity.PayerId = medicationItem.PayerId;
        entity.EncounterId = medicationItem.EncounterId;
        entity.Code = medicationItem.Code;
        entity.Description = medicationItem.Description;
        entity.BaseCost = medicationItem.BaseCost;
        entity.PayerCoverage = medicationItem.PayerCoverage;
        entity.TotalCost = medicationItem.TotalCost;
        entity.Dispenses = medicationItem.Dispenses;
        entity.ReasonCode = medicationItem.ReasonCode;
        entity.ReasonDescription = medicationItem.ReasonDescription;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(long id)
    {
        var entity = await _context.Medications.FirstOrDefaultAsync(m => m.Id == id);
        if (entity is null) return false;

        _context.Medications.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
