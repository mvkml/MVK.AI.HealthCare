using AI.HealthCare.Patient.Models.Allergy;
using EfAllergy = AI.HealthCare.Patient.EF.Entities.Allergy;

namespace AI.HealthCare.Patient.Repositories;

public class AllergyMapper : IAllergyMapper
{
    public AllergyItem ToModel(EfAllergy entity) => new()
    {
        Id = entity.Id,
        Start = entity.Start,
        Stop = entity.Stop,
        PatientId = entity.PatientId,
        EncounterId = entity.EncounterId,
        Code = entity.Code,
        System = entity.System,
        Description = entity.Description,
        Type = entity.Type,
        Category = entity.Category,
        Reaction1 = entity.Reaction1,
        Description1 = entity.Description1,
        Severity1 = entity.Severity1,
        Reaction2 = entity.Reaction2,
        Description2 = entity.Description2,
        Severity2 = entity.Severity2
    };

    public EfAllergy ToEntity(AllergyItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        Code = item.Code,
        System = item.System,
        Description = item.Description,
        Type = item.Type,
        Category = item.Category,
        Reaction1 = item.Reaction1,
        Description1 = item.Description1,
        Severity1 = item.Severity1,
        Reaction2 = item.Reaction2,
        Description2 = item.Description2,
        Severity2 = item.Severity2
    };
}
