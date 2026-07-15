using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.Patient;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly PatientDbContext _context;
    private readonly IPatientMapper _mapper;

    public PatientRepository(PatientDbContext context, IPatientMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PatientItem?> GetById(Guid id)
    {
        var entity = await _context.Patients.FirstOrDefaultAsync(p => p.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<PatientItem>> GetAll()
    {
        var entities = await _context.Patients.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<PatientItem> Create(PatientItem patientItem)
    {
        var entity = _mapper.ToEntity(patientItem);
        _context.Patients.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task CreateBatch(List<PatientItem> patientItems)
    {
        var entities = patientItems.Select(_mapper.ToEntity);
        _context.Patients.AddRange(entities);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task<PatientItem?> Update(PatientItem patientItem)
    {
        var entity = await _context.Patients.FirstOrDefaultAsync(p => p.Id == patientItem.Id);
        if (entity is null) return null;

        entity.BirthDate = patientItem.BirthDate;
        entity.DeathDate = patientItem.DeathDate;
        entity.Ssn = patientItem.Ssn;
        entity.Drivers = patientItem.Drivers;
        entity.Passport = patientItem.Passport;
        entity.Prefix = patientItem.Prefix;
        entity.First = patientItem.First;
        entity.Middle = patientItem.Middle;
        entity.Last = patientItem.Last;
        entity.Suffix = patientItem.Suffix;
        entity.Maiden = patientItem.Maiden;
        entity.Marital = patientItem.Marital;
        entity.Race = patientItem.Race;
        entity.Ethnicity = patientItem.Ethnicity;
        entity.Gender = patientItem.Gender;
        entity.Birthplace = patientItem.Birthplace;
        entity.Address = patientItem.Address;
        entity.City = patientItem.City;
        entity.State = patientItem.State;
        entity.County = patientItem.County;
        entity.Fips = patientItem.Fips;
        entity.Zip = patientItem.Zip;
        entity.Lat = patientItem.Lat;
        entity.Lon = patientItem.Lon;
        entity.HealthcareExpenses = patientItem.HealthcareExpenses;
        entity.HealthcareCoverage = patientItem.HealthcareCoverage;
        entity.Income = patientItem.Income;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await _context.Patients.FirstOrDefaultAsync(p => p.Id == id);
        if (entity is null) return false;

        _context.Patients.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
