using AI.HealthCare.Patient.Models.Provider;
using EfProvider = AI.HealthCare.Patient.EF.Entities.Provider;

namespace AI.HealthCare.Patient.Repositories;

public class ProviderMapper : IProviderMapper
{
    public ProviderItem ToModel(EfProvider entity) => new()
    {
        Id = entity.Id,
        OrganizationId = entity.OrganizationId,
        Name = entity.Name,
        Gender = entity.Gender,
        Speciality = entity.Speciality,
        Address = entity.Address,
        City = entity.City,
        State = entity.State,
        Zip = entity.Zip,
        Lat = entity.Lat,
        Lon = entity.Lon,
        Encounters = entity.Encounters,
        Procedures = entity.Procedures
    };

    public EfProvider ToEntity(ProviderItem item) => new()
    {
        Id = item.Id,
        OrganizationId = item.OrganizationId,
        Name = item.Name,
        Gender = item.Gender,
        Speciality = item.Speciality,
        Address = item.Address,
        City = item.City,
        State = item.State,
        Zip = item.Zip,
        Lat = item.Lat,
        Lon = item.Lon,
        Encounters = item.Encounters,
        Procedures = item.Procedures
    };
}
