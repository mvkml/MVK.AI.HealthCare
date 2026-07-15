using AI.HealthCare.Patient.Models.Allergy;
using AI.HealthCare.Patient.Models.Shared;

namespace AI.HealthCare.Patient.BL;

public interface IAllergyBL
{
    Task<AllergiesModel> Create(AllergiesModel allergiesModel);
    Task<AllergiesModel> GetById(AllergiesModel allergiesModel);
    Task<AllergiesModel> GetAll(AllergiesModel allergiesModel);
    Task<AllergiesModel> GetByPatientId(Guid patientId);
    Task<AllergiesModel> Update(AllergiesModel allergiesModel);
    Task<AllergiesModel> Delete(AllergiesModel allergiesModel);
    Task<ImportResult> Import(Stream csvStream);
    Task<ImportResult> ImportUpsert(Stream csvStream);
}
