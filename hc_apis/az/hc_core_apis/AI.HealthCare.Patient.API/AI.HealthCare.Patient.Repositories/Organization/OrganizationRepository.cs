using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Models.Organization;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly PatientDbContext _context;
    private readonly IOrganizationMapper _mapper;

    public OrganizationRepository(PatientDbContext context, IOrganizationMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrganizationItem?> GetById(Guid id)
    {
        var entity = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == id);
        return entity is null ? null : _mapper.ToModel(entity);
    }

    public async Task<List<OrganizationItem>> GetAll()
    {
        var entities = await _context.Organizations.ToListAsync();
        return entities.Select(_mapper.ToModel).ToList();
    }

    public async Task<OrganizationItem> Create(OrganizationItem organizationItem)
    {
        var entity = _mapper.ToEntity(organizationItem);
        _context.Organizations.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<OrganizationItem?> Update(OrganizationItem organizationItem)
    {
        var entity = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == organizationItem.Id);
        if (entity is null) return null;

        entity.Name = organizationItem.Name;
        entity.Address = organizationItem.Address;
        entity.City = organizationItem.City;
        entity.State = organizationItem.State;
        entity.Zip = organizationItem.Zip;
        entity.Phone = organizationItem.Phone;
        entity.Lat = organizationItem.Lat;
        entity.Lon = organizationItem.Lon;
        entity.Revenue = organizationItem.Revenue;
        entity.Utilization = organizationItem.Utilization;

        await _context.SaveChangesAsync();
        return _mapper.ToModel(entity);
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == id);
        if (entity is null) return false;

        _context.Organizations.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
