using AI.HealthCare.Patient.Models.ClaimTransaction;
using EfClaimTransaction = AI.HealthCare.Patient.EF.Entities.ClaimTransaction;

namespace AI.HealthCare.Patient.Repositories;

public class ClaimTransactionMapper : IClaimTransactionMapper
{
    public ClaimTransactionItem ToModel(EfClaimTransaction entity) => new()
    {
        Id = entity.Id,
        ClaimId = entity.ClaimId,
        ChargeId = entity.ChargeId,
        PatientId = entity.PatientId,
        Type = entity.Type,
        Amount = entity.Amount,
        Method = entity.Method,
        FromDate = entity.FromDate,
        ToDate = entity.ToDate,
        PlaceOfServiceId = entity.PlaceOfServiceId,
        ProcedureCode = entity.ProcedureCode,
        Modifier1 = entity.Modifier1,
        Modifier2 = entity.Modifier2,
        DiagnosisRef1 = entity.DiagnosisRef1,
        DiagnosisRef2 = entity.DiagnosisRef2,
        DiagnosisRef3 = entity.DiagnosisRef3,
        DiagnosisRef4 = entity.DiagnosisRef4,
        Units = entity.Units,
        DepartmentId = entity.DepartmentId,
        Notes = entity.Notes,
        UnitAmount = entity.UnitAmount,
        TransferOutId = entity.TransferOutId,
        TransferType = entity.TransferType,
        Payments = entity.Payments,
        Adjustments = entity.Adjustments,
        Transfers = entity.Transfers,
        Outstanding = entity.Outstanding,
        AppointmentId = entity.AppointmentId,
        LineNote = entity.LineNote,
        PatientInsuranceId = entity.PatientInsuranceId,
        FeeScheduleId = entity.FeeScheduleId,
        ProviderId = entity.ProviderId,
        SupervisingProviderId = entity.SupervisingProviderId
    };

    public EfClaimTransaction ToEntity(ClaimTransactionItem item) => new()
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
