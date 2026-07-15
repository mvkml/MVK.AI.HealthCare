using AI.HealthCare.Patient.Models.Patient;
using EfPatient = AI.HealthCare.Patient.EF.Entities.Patient;

namespace AI.HealthCare.Patient.Repositories;

public interface IPatientMapper
{
    PatientItem ToModel(EfPatient entity);
    EfPatient ToEntity(PatientItem item);
}
