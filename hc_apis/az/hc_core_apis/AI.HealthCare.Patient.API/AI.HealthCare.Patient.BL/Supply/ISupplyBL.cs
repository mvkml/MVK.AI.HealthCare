using AI.HealthCare.Patient.Models.Supply;

namespace AI.HealthCare.Patient.BL;

public interface ISupplyBL
{
    Task<SuppliesModel> Create(SuppliesModel suppliesModel);
    Task<SuppliesModel> GetById(SuppliesModel suppliesModel);
    Task<SuppliesModel> GetAll(SuppliesModel suppliesModel);
    Task<SuppliesModel> GetByPatientId(Guid patientId);
    Task<SuppliesModel> Update(SuppliesModel suppliesModel);
    Task<SuppliesModel> Delete(SuppliesModel suppliesModel);
}
