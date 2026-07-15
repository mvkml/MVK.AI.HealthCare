using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.Observation;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class ObservationRepository : IObservationRepository
{
    private readonly PatientDbContext _context;
    private readonly IObservationMapper _mapper;

    public ObservationRepository(PatientDbContext context, IObservationMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ObservationItem?> GetById(long id)
    {
        var entity = await _context.Observations.FirstOrDefaultAsync(o => o.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<ObservationItem>> GetAll()
    {
        var entities = await _context.Observations.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<List<ObservationItem>> GetByPatientId(Guid patientId)
    {
        var entities = await _context.Observations.Where(o => o.PatientId == patientId).ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<ObservationItem> Create(ObservationItem observationItem)
    {
        var entity = _mapper.ToEntity(observationItem);
        _context.Observations.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<ObservationItem?> Update(ObservationItem observationItem)
    {
        var entity = await _context.Observations.FirstOrDefaultAsync(o => o.Id == observationItem.Id);
        if (entity is null) return null;

        entity.Date = observationItem.Date;
        entity.PatientId = observationItem.PatientId;
        entity.EncounterId = observationItem.EncounterId;
        entity.Category = observationItem.Category;
        entity.Code = observationItem.Code;
        entity.Description = observationItem.Description;
        entity.Value = observationItem.Value;
        entity.Units = observationItem.Units;
        entity.Type = observationItem.Type;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(long id)
    {
        var entity = await _context.Observations.FirstOrDefaultAsync(o => o.Id == id);
        if (entity is null) return false;

        _context.Observations.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
