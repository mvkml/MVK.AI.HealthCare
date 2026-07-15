using AI.HealthCare.Patient.Models.ClaimTransaction;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class ClaimTransactionBL : IClaimTransactionBL
{
    private readonly IClaimTransactionRepository _claimTransactionRepository;

    public ClaimTransactionBL(IClaimTransactionRepository claimTransactionRepository)
    {
        _claimTransactionRepository = claimTransactionRepository;
    }

    public async Task<ClaimTransactionsModel> Create(ClaimTransactionsModel claimTransactionsModel)
    {
        claimTransactionsModel.ClaimTransactionItem = ToItem(claimTransactionsModel.ClaimTransactionRequest);
        claimTransactionsModel.ClaimTransactionItem.Id = Guid.NewGuid();

        var savedItem = await _claimTransactionRepository.Create(claimTransactionsModel.ClaimTransactionItem);
        claimTransactionsModel.ClaimTransactionItem = savedItem;

        claimTransactionsModel.ClaimTransactionResponse = ToResponse(savedItem);
        claimTransactionsModel.IsNotValid = false;
        claimTransactionsModel.Message = "ClaimTransaction created successfully.";

        return claimTransactionsModel;
    }

    public async Task<ClaimTransactionsModel> GetById(ClaimTransactionsModel claimTransactionsModel)
    {
        var item = await _claimTransactionRepository.GetById(claimTransactionsModel.ClaimTransactionItem.Id);
        if (item is null)
        {
            claimTransactionsModel.IsNotValid = true;
            claimTransactionsModel.Message = "ClaimTransaction not found.";
            return claimTransactionsModel;
        }

        claimTransactionsModel.ClaimTransactionItem = item;
        claimTransactionsModel.ClaimTransactionResponse = ToResponse(item);
        claimTransactionsModel.IsNotValid = false;
        claimTransactionsModel.Message = "ClaimTransaction retrieved successfully.";
        return claimTransactionsModel;
    }

    public async Task<ClaimTransactionsModel> GetAll(ClaimTransactionsModel claimTransactionsModel)
    {
        var items = await _claimTransactionRepository.GetAll();
        claimTransactionsModel.ClaimTransactionItems = items;
        claimTransactionsModel.IsNotValid = false;
        claimTransactionsModel.Message = $"{items.Count} claim transaction(s) retrieved.";
        return claimTransactionsModel;
    }

    public async Task<ClaimTransactionsModel> GetByClaimId(Guid claimId)
    {
        var items = await _claimTransactionRepository.GetByClaimId(claimId);
        return new ClaimTransactionsModel
        {
            ClaimTransactionItems = items,
            IsNotValid = false,
            Message = $"{items.Count} claim transaction(s) retrieved for claim."
        };
    }

    public async Task<ClaimTransactionsModel> Update(ClaimTransactionsModel claimTransactionsModel)
    {
        var existing = await _claimTransactionRepository.GetById(claimTransactionsModel.ClaimTransactionItem.Id);
        if (existing is null)
        {
            claimTransactionsModel.IsNotValid = true;
            claimTransactionsModel.Message = "ClaimTransaction not found.";
            return claimTransactionsModel;
        }

        var updatedItem = ToItem(claimTransactionsModel.ClaimTransactionRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _claimTransactionRepository.Update(updatedItem);
        claimTransactionsModel.ClaimTransactionItem = savedItem!;
        claimTransactionsModel.ClaimTransactionResponse = ToResponse(savedItem!);
        claimTransactionsModel.IsNotValid = false;
        claimTransactionsModel.Message = "ClaimTransaction updated successfully.";
        return claimTransactionsModel;
    }

    public async Task<ClaimTransactionsModel> Delete(ClaimTransactionsModel claimTransactionsModel)
    {
        var deleted = await _claimTransactionRepository.Delete(claimTransactionsModel.ClaimTransactionItem.Id);
        if (!deleted)
        {
            claimTransactionsModel.IsNotValid = true;
            claimTransactionsModel.Message = "ClaimTransaction not found.";
            return claimTransactionsModel;
        }

        claimTransactionsModel.IsNotValid = false;
        claimTransactionsModel.Message = "ClaimTransaction deleted successfully.";
        return claimTransactionsModel;
    }

    private static ClaimTransactionItem ToItem(ClaimTransactionRequest request) => new()
    {
        ClaimId = request.ClaimId,
        ChargeId = request.ChargeId,
        PatientId = request.PatientId,
        Type = request.Type,
        Amount = request.Amount,
        Method = request.Method,
        FromDate = request.FromDate,
        ToDate = request.ToDate,
        PlaceOfServiceId = request.PlaceOfServiceId,
        ProcedureCode = request.ProcedureCode,
        Modifier1 = request.Modifier1,
        Modifier2 = request.Modifier2,
        DiagnosisRef1 = request.DiagnosisRef1,
        DiagnosisRef2 = request.DiagnosisRef2,
        DiagnosisRef3 = request.DiagnosisRef3,
        DiagnosisRef4 = request.DiagnosisRef4,
        Units = request.Units,
        DepartmentId = request.DepartmentId,
        Notes = request.Notes,
        UnitAmount = request.UnitAmount,
        TransferOutId = request.TransferOutId,
        TransferType = request.TransferType,
        Payments = request.Payments,
        Adjustments = request.Adjustments,
        Transfers = request.Transfers,
        Outstanding = request.Outstanding,
        AppointmentId = request.AppointmentId,
        LineNote = request.LineNote,
        PatientInsuranceId = request.PatientInsuranceId,
        FeeScheduleId = request.FeeScheduleId,
        ProviderId = request.ProviderId,
        SupervisingProviderId = request.SupervisingProviderId
    };

    private static ClaimTransactionResponse ToResponse(ClaimTransactionItem item) => new()
    {
        Id = item.Id,
        ClaimId = item.ClaimId,
        ChargeId = item.ChargeId,
        PatientId = item.PatientId,
        Type = item.Type,
        Amount = item.Amount,
        Method = item.Method,
        FromDate = item.FromDate,
        ToDate = item.ToDate,
        PlaceOfServiceId = item.PlaceOfServiceId,
        ProcedureCode = item.ProcedureCode,
        Modifier1 = item.Modifier1,
        Modifier2 = item.Modifier2,
        DiagnosisRef1 = item.DiagnosisRef1,
        DiagnosisRef2 = item.DiagnosisRef2,
        DiagnosisRef3 = item.DiagnosisRef3,
        DiagnosisRef4 = item.DiagnosisRef4,
        Units = item.Units,
        DepartmentId = item.DepartmentId,
        Notes = item.Notes,
        UnitAmount = item.UnitAmount,
        TransferOutId = item.TransferOutId,
        TransferType = item.TransferType,
        Payments = item.Payments,
        Adjustments = item.Adjustments,
        Transfers = item.Transfers,
        Outstanding = item.Outstanding,
        AppointmentId = item.AppointmentId,
        LineNote = item.LineNote,
        PatientInsuranceId = item.PatientInsuranceId,
        FeeScheduleId = item.FeeScheduleId,
        ProviderId = item.ProviderId,
        SupervisingProviderId = item.SupervisingProviderId
    };
}
