using AI.HealthCare.Patient.Models.PayerTransition;
using AI.HealthCare.Patient.Models.Shared;

namespace AI.HealthCare.Patient.BL;

public interface IPayerTransitionBL
{
    Task<PayerTransitionsModel> Create(PayerTransitionsModel payerTransitionsModel);
    Task<PayerTransitionsModel> GetById(PayerTransitionsModel payerTransitionsModel);
    Task<PayerTransitionsModel> GetAll(PayerTransitionsModel payerTransitionsModel);
    Task<PayerTransitionsModel> Update(PayerTransitionsModel payerTransitionsModel);
    Task<PayerTransitionsModel> Delete(PayerTransitionsModel payerTransitionsModel);
    Task<ImportResult> Import(Stream csvStream);
}
