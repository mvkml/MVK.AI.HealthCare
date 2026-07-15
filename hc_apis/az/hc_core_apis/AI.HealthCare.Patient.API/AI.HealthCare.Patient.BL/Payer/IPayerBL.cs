using AI.HealthCare.Patient.Models.Payer;
using AI.HealthCare.Patient.Models.Shared;

namespace AI.HealthCare.Patient.BL;

public interface IPayerBL
{
    Task<PayersModel> Create(PayersModel payersModel);
    Task<PayersModel> GetById(PayersModel payersModel);
    Task<PayersModel> GetAll(PayersModel payersModel);
    Task<PayersModel> Update(PayersModel payersModel);
    Task<PayersModel> Delete(PayersModel payersModel);
    Task<ImportResult> Import(Stream csvStream);
}
