using AI.HealthCare.Patient.Models.PayerTransition;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class PayerTransitionBL : IPayerTransitionBL
{
    private readonly IPayerTransitionRepository _payerTransitionRepository;

    public PayerTransitionBL(IPayerTransitionRepository payerTransitionRepository)
    {
        _payerTransitionRepository = payerTransitionRepository;
    }

    public async Task<PayerTransitionsModel> Create(PayerTransitionsModel payerTransitionsModel)
    {
        payerTransitionsModel.PayerTransitionItem = ToItem(payerTransitionsModel.PayerTransitionRequest);

        var savedItem = await _payerTransitionRepository.Create(payerTransitionsModel.PayerTransitionItem);
        payerTransitionsModel.PayerTransitionItem = savedItem;

        payerTransitionsModel.PayerTransitionResponse = ToResponse(savedItem);
        payerTransitionsModel.IsNotValid = false;
        payerTransitionsModel.Message = "PayerTransition created successfully.";

        return payerTransitionsModel;
    }

    public async Task<PayerTransitionsModel> GetById(PayerTransitionsModel payerTransitionsModel)
    {
        var item = await _payerTransitionRepository.GetById(payerTransitionsModel.PayerTransitionItem.Id);
        if (item is null)
        {
            payerTransitionsModel.IsNotValid = true;
            payerTransitionsModel.Message = "PayerTransition not found.";
            return payerTransitionsModel;
        }

        payerTransitionsModel.PayerTransitionItem = item;
        payerTransitionsModel.PayerTransitionResponse = ToResponse(item);
        payerTransitionsModel.IsNotValid = false;
        payerTransitionsModel.Message = "PayerTransition retrieved successfully.";
        return payerTransitionsModel;
    }

    public async Task<PayerTransitionsModel> GetAll(PayerTransitionsModel payerTransitionsModel)
    {
        var items = await _payerTransitionRepository.GetAll();
        payerTransitionsModel.PayerTransitionItems = items;
        payerTransitionsModel.IsNotValid = false;
        payerTransitionsModel.Message = $"{items.Count} payer transition(s) retrieved.";
        return payerTransitionsModel;
    }

    public async Task<PayerTransitionsModel> Update(PayerTransitionsModel payerTransitionsModel)
    {
        var existing = await _payerTransitionRepository.GetById(payerTransitionsModel.PayerTransitionItem.Id);
        if (existing is null)
        {
            payerTransitionsModel.IsNotValid = true;
            payerTransitionsModel.Message = "PayerTransition not found.";
            return payerTransitionsModel;
        }

        var updatedItem = ToItem(payerTransitionsModel.PayerTransitionRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _payerTransitionRepository.Update(updatedItem);
        payerTransitionsModel.PayerTransitionItem = savedItem!;
        payerTransitionsModel.PayerTransitionResponse = ToResponse(savedItem!);
        payerTransitionsModel.IsNotValid = false;
        payerTransitionsModel.Message = "PayerTransition updated successfully.";
        return payerTransitionsModel;
    }

    public async Task<PayerTransitionsModel> Delete(PayerTransitionsModel payerTransitionsModel)
    {
        var deleted = await _payerTransitionRepository.Delete(payerTransitionsModel.PayerTransitionItem.Id);
        if (!deleted)
        {
            payerTransitionsModel.IsNotValid = true;
            payerTransitionsModel.Message = "PayerTransition not found.";
            return payerTransitionsModel;
        }

        payerTransitionsModel.IsNotValid = false;
        payerTransitionsModel.Message = "PayerTransition deleted successfully.";
        return payerTransitionsModel;
    }

    private static PayerTransitionItem ToItem(PayerTransitionRequest request) => new()
    {
        PatientId = request.PatientId,
        MemberId = request.MemberId,
        StartDate = request.StartDate,
        EndDate = request.EndDate,
        PayerId = request.PayerId,
        SecondaryPayerId = request.SecondaryPayerId,
        PlanOwnership = request.PlanOwnership,
        OwnerName = request.OwnerName
    };

    private static PayerTransitionResponse ToResponse(PayerTransitionItem item) => new()
    {
        Id = item.Id,
        PatientId = item.PatientId,
        MemberId = item.MemberId,
        StartDate = item.StartDate,
        EndDate = item.EndDate,
        PayerId = item.PayerId,
        SecondaryPayerId = item.SecondaryPayerId,
        PlanOwnership = item.PlanOwnership,
        OwnerName = item.OwnerName
    };
}
