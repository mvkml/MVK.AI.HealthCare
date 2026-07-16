using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.ImagingStudy;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class ImagingStudyRepository : IImagingStudyRepository
{
    private readonly PatientDbContext _context;
    private readonly IImagingStudyMapper _mapper;

    public ImagingStudyRepository(PatientDbContext context, IImagingStudyMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ImagingStudyItem?> GetById(long id)
    {
        var entity = await _context.ImagingStudies.FirstOrDefaultAsync(i => i.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<ImagingStudyItem>> GetAll()
    {
        var entities = await _context.ImagingStudies.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<List<ImagingStudyItem>> GetByPatientId(Guid patientId)
    {
        var entities = await _context.ImagingStudies.Where(i => i.PatientId == patientId).ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<ImagingStudyItem> Create(ImagingStudyItem imagingStudyItem)
    {
        var entity = _mapper.ToEntity(imagingStudyItem);
        _context.ImagingStudies.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task CreateBatch(List<ImagingStudyItem> imagingStudyItems)
    {
        var entities = imagingStudyItems.Select(_mapper.ToEntity);
        _context.ImagingStudies.AddRange(entities);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task UpsertBatch(List<ImagingStudyItem> imagingStudyItems)
    {
        var instanceUids = imagingStudyItems.Select(i => i.InstanceUid).ToHashSet();

        var existingEntities = await _context.ImagingStudies
            .Where(i => instanceUids.Contains(i.InstanceUid))
            .ToListAsync();
        var existingByKey = existingEntities.ToLookup(e => e.InstanceUid);

        foreach (var item in imagingStudyItems)
        {
            var match = existingByKey[item.InstanceUid].FirstOrDefault();
            if (match is not null)
            {
                match.StudyId = item.StudyId;
                match.Date = item.Date;
                match.PatientId = item.PatientId;
                match.EncounterId = item.EncounterId;
                match.SeriesUid = item.SeriesUid;
                match.BodysiteCode = item.BodysiteCode;
                match.BodysiteDescription = item.BodysiteDescription;
                match.ModalityCode = item.ModalityCode;
                match.ModalityDescription = item.ModalityDescription;
                match.SopCode = item.SopCode;
                match.SopDescription = item.SopDescription;
                match.ProcedureCode = item.ProcedureCode;
            }
            else
            {
                _context.ImagingStudies.Add(_mapper.ToEntity(item));
            }
        }

        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task<ImagingStudyItem?> Update(ImagingStudyItem imagingStudyItem)
    {
        var entity = await _context.ImagingStudies.FirstOrDefaultAsync(i => i.Id == imagingStudyItem.Id);
        if (entity is null) return null;

        entity.StudyId = imagingStudyItem.StudyId;
        entity.Date = imagingStudyItem.Date;
        entity.PatientId = imagingStudyItem.PatientId;
        entity.EncounterId = imagingStudyItem.EncounterId;
        entity.SeriesUid = imagingStudyItem.SeriesUid;
        entity.BodysiteCode = imagingStudyItem.BodysiteCode;
        entity.BodysiteDescription = imagingStudyItem.BodysiteDescription;
        entity.ModalityCode = imagingStudyItem.ModalityCode;
        entity.ModalityDescription = imagingStudyItem.ModalityDescription;
        entity.InstanceUid = imagingStudyItem.InstanceUid;
        entity.SopCode = imagingStudyItem.SopCode;
        entity.SopDescription = imagingStudyItem.SopDescription;
        entity.ProcedureCode = imagingStudyItem.ProcedureCode;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(long id)
    {
        var entity = await _context.ImagingStudies.FirstOrDefaultAsync(i => i.Id == id);
        if (entity is null) return false;

        _context.ImagingStudies.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
