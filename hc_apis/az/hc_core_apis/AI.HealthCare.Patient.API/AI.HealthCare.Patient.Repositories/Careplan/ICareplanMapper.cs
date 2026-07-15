using AI.HealthCare.Patient.Models.Careplan;
using EfCareplan = AI.HealthCare.Patient.EF.Entities.Careplan;

namespace AI.HealthCare.Patient.Repositories;

public interface ICareplanMapper
{
    CareplanItem ToModel(EfCareplan entity);
    EfCareplan ToEntity(CareplanItem item);
}
