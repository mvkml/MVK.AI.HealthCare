using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.Provider;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class ProviderRepository : IProviderRepository
{
    private readonly PatientDbContext _context;
    private readonly IProviderMapper _mapper;

    public ProviderRepository(PatientDbContext context, IProviderMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProviderItem?> GetById(Guid id)
    {
        var entity = await _context.Providers.FirstOrDefaultAsync(p => p.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<ProviderItem>> GetAll()
    {
        var entities = await _context.Providers.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<ProviderItem> Create(ProviderItem providerItem)
    {
        var entity = _mapper.ToEntity(providerItem);
        _context.Providers.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<ProviderItem?> Update(ProviderItem providerItem)
    {
        var entity = await _context.Providers.FirstOrDefaultAsync(p => p.Id == providerItem.Id);
        if (entity is null) return null;

        entity.OrganizationId = providerItem.OrganizationId;
        entity.Name = providerItem.Name;
        entity.Gender = providerItem.Gender;
        entity.Speciality = providerItem.Speciality;
        entity.Address = providerItem.Address;
        entity.City = providerItem.City;
        entity.State = providerItem.State;
        entity.Zip = providerItem.Zip;
        entity.Lat = providerItem.Lat;
        entity.Lon = providerItem.Lon;
        entity.Encounters = providerItem.Encounters;
        entity.Procedures = providerItem.Procedures;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await _context.Providers.FirstOrDefaultAsync(p => p.Id == id);
        if (entity is null) return false;

        _context.Providers.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
