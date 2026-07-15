using AI.HealthCare.Patient.Models.Payer;

namespace AI.HealthCare.Patient.BL;

public interface IPayerBL
{
    Task<PayersModel> Create(PayersModel payersModel);
    Task<PayersModel> GetById(PayersModel payersModel);
    Task<PayersModel> GetAll(PayersModel payersModel);
    Task<PayersModel> Update(PayersModel payersModel);
    Task<PayersModel> Delete(PayersModel payersModel);
}
