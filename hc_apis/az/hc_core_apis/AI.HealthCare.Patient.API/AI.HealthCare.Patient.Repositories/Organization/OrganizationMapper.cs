using AI.HealthCare.Patient.Models.Organization;
using EfOrganization = AI.HealthCare.Patient.EF.Entities.Organization;

namespace AI.HealthCare.Patient.Repositories;

public class OrganizationMapper : IOrganizationMapper
{
    public OrganizationItem ToModel(EfOrganization entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Address = entity.Address,
        City = entity.City,
        State = entity.State,
        Zip = entity.Zip,
        Phone = entity.Phone,
        Lat = entity.Lat,
        Lon = entity.Lon,
        Revenue = entity.Revenue,
        Utilization = entity.Utilization
    };

    public EfOrganization ToEntity(OrganizationItem item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        Address = item.Address,
        City = item.City,
        State = item.State,
        Zip = item.Zip,
        Phone = item.Phone,
        Lat = item.Lat,
        Lon = item.Lon,
        Revenue = item.Revenue,
        Utilization = item.Utilization
    };
}
