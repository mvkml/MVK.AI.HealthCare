using AI.HealthCare.Patient.Models.Immunization;

namespace AI.HealthCare.Patient.BL;

public interface IImmunizationBL
{
    Task<ImmunizationsModel> Create(ImmunizationsModel immunizationsModel);
    Task<ImmunizationsModel> GetById(ImmunizationsModel immunizationsModel);
    Task<ImmunizationsModel> GetAll(ImmunizationsModel immunizationsModel);
    Task<ImmunizationsModel> GetByPatientId(Guid patientId);
    Task<ImmunizationsModel> Update(ImmunizationsModel immunizationsModel);
    Task<ImmunizationsModel> Delete(ImmunizationsModel immunizationsModel);
}
