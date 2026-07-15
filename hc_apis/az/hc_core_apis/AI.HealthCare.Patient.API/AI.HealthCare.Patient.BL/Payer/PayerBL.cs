using AI.HealthCare.Patient.Models.Payer;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class PayerBL : IPayerBL
{
    private readonly IPayerRepository _payerRepository;

    public PayerBL(IPayerRepository payerRepository)
    {
        _payerRepository = payerRepository;
    }

    public async Task<PayersModel> Create(PayersModel payersModel)
    {
        payersModel.PayerItem = ToItem(payersModel.PayerRequest);
        payersModel.PayerItem.Id = Guid.NewGuid();

        var savedItem = await _payerRepository.Create(payersModel.PayerItem);
        payersModel.PayerItem = savedItem;

        payersModel.PayerResponse = ToResponse(savedItem);
        payersModel.IsNotValid = false;
        payersModel.Message = "Payer created successfully.";

        return payersModel;
    }

    public async Task<PayersModel> GetById(PayersModel payersModel)
    {
        var item = await _payerRepository.GetById(payersModel.PayerItem.Id);
        if (item is null)
        {
            payersModel.IsNotValid = true;
            payersModel.Message = "Payer not found.";
            return payersModel;
        }

        payersModel.PayerItem = item;
        payersModel.PayerResponse = ToResponse(item);
        payersModel.IsNotValid = false;
        payersModel.Message = "Payer retrieved successfully.";
        return payersModel;
    }

    public async Task<PayersModel> GetAll(PayersModel payersModel)
    {
        var items = await _payerRepository.GetAll();
        payersModel.PayerItems = items;
        payersModel.IsNotValid = false;
        payersModel.Message = $"{items.Count} payer(s) retrieved.";
        return payersModel;
    }

    public async Task<PayersModel> Update(PayersModel payersModel)
    {
        var existing = await _payerRepository.GetById(payersModel.PayerItem.Id);
        if (existing is null)
        {
            payersModel.IsNotValid = true;
            payersModel.Message = "Payer not found.";
            return payersModel;
        }

        var updatedItem = ToItem(payersModel.PayerRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _payerRepository.Update(updatedItem);
        payersModel.PayerItem = savedItem!;
        payersModel.PayerResponse = ToResponse(savedItem!);
        payersModel.IsNotValid = false;
        payersModel.Message = "Payer updated successfully.";
        return payersModel;
    }

    public async Task<PayersModel> Delete(PayersModel payersModel)
    {
        var deleted = await _payerRepository.Delete(payersModel.PayerItem.Id);
        if (!deleted)
        {
            payersModel.IsNotValid = true;
            payersModel.Message = "Payer not found.";
            return payersModel;
        }

        payersModel.IsNotValid = false;
        payersModel.Message = "Payer deleted successfully.";
        return payersModel;
    }

    private static PayerItem ToItem(PayerRequest request) => new()
    {
        Name = request.Name,
        Ownership = request.Ownership,
        Address = request.Address,
        City = request.City,
        StateHeadquartered = request.StateHeadquartered,
        Zip = request.Zip,
        Phone = request.Phone,
        AmountCovered = request.AmountCovered,
        AmountUncovered = request.AmountUncovered,
        Revenue = request.Revenue,
        CoveredEncounters = request.CoveredEncounters,
        UncoveredEncounters = request.UncoveredEncounters,
        CoveredMedications = request.CoveredMedications,
        UncoveredMedications = request.UncoveredMedications,
        CoveredProcedures = request.CoveredProcedures,
        UncoveredProcedures = request.UncoveredProcedures,
        CoveredImmunizations = request.CoveredImmunizations,
        UncoveredImmunizations = request.UncoveredImmunizations,
        UniqueCustomers = request.UniqueCustomers,
        QolsAvg = request.QolsAvg,
        MemberMonths = request.MemberMonths
    };

    private static PayerResponse ToResponse(PayerItem item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        Ownership = item.Ownership,
        Address = item.Address,
        City = item.City,
        StateHeadquartered = item.StateHeadquartered,
        Zip = item.Zip,
        Phone = item.Phone,
        AmountCovered = item.AmountCovered,
        AmountUncovered = item.AmountUncovered,
        Revenue = item.Revenue,
        CoveredEncounters = item.CoveredEncounters,
        UncoveredEncounters = item.UncoveredEncounters,
        CoveredMedications = item.CoveredMedications,
        UncoveredMedications = item.UncoveredMedications,
        CoveredProcedures = item.CoveredProcedures,
        UncoveredProcedures = item.UncoveredProcedures,
        CoveredImmunizations = item.CoveredImmunizations,
        UncoveredImmunizations = item.UncoveredImmunizations,
        UniqueCustomers = item.UniqueCustomers,
        QolsAvg = item.QolsAvg,
        MemberMonths = item.MemberMonths
    };
}
