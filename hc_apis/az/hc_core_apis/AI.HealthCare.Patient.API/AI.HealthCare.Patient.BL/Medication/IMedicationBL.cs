using AI.HealthCare.Patient.Models.Medication;

namespace AI.HealthCare.Patient.BL;

public interface IMedicationBL
{
    Task<MedicationsModel> Create(MedicationsModel medicationsModel);
    Task<MedicationsModel> GetById(MedicationsModel medicationsModel);
    Task<MedicationsModel> GetAll(MedicationsModel medicationsModel);
    Task<MedicationsModel> GetByPatientId(Guid patientId);
    Task<MedicationsModel> Update(MedicationsModel medicationsModel);
    Task<MedicationsModel> Delete(MedicationsModel medicationsModel);
}
