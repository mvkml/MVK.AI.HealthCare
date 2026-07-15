using AI.HealthCare.Patient.Models.Supply;
using AI.HealthCare.Patient.Models.Shared;

namespace AI.HealthCare.Patient.BL;

public interface ISupplyBL
{
    Task<SuppliesModel> Create(SuppliesModel suppliesModel);
    Task<SuppliesModel> GetById(SuppliesModel suppliesModel);
    Task<SuppliesModel> GetAll(SuppliesModel suppliesModel);
    Task<SuppliesModel> GetByPatientId(Guid patientId);
    Task<SuppliesModel> Update(SuppliesModel suppliesModel);
    Task<SuppliesModel> Delete(SuppliesModel suppliesModel);
    Task<ImportResult> Import(Stream csvStream);
    Task<ImportResult> ImportUpsert(Stream csvStream);
}
