using AI.HealthCare.Patient.Models.Allergy;

namespace AI.HealthCare.Patient.BL;

public interface IAllergyBL
{
    Task<AllergiesModel> Create(AllergiesModel allergiesModel);
    Task<AllergiesModel> GetById(AllergiesModel allergiesModel);
    Task<AllergiesModel> GetAll(AllergiesModel allergiesModel);
    Task<AllergiesModel> GetByPatientId(Guid patientId);
    Task<AllergiesModel> Update(AllergiesModel allergiesModel);
    Task<AllergiesModel> Delete(AllergiesModel allergiesModel);
}
