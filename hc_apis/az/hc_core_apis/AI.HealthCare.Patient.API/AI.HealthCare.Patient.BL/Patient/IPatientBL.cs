using AI.HealthCare.Patient.Models.Patient;

namespace AI.HealthCare.Patient.BL;

public interface IPatientBL
{
    Task<PatientsModel> Create(PatientsModel patientsModel);
    Task<PatientsModel> GetById(PatientsModel patientsModel);
    Task<PatientsModel> GetAll(PatientsModel patientsModel);
    Task<PatientsModel> Update(PatientsModel patientsModel);
    Task<PatientsModel> Delete(PatientsModel patientsModel);
}
