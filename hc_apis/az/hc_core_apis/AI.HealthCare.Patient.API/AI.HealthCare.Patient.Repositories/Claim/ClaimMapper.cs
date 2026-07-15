using AI.HealthCare.Patient.Models.Claim;
using EfClaim = AI.HealthCare.Patient.EF.Entities.Claim;

namespace AI.HealthCare.Patient.Repositories;

public class ClaimMapper : IClaimMapper
{
    public ClaimItem ToModel(EfClaim entity) => new()
    {
        Id = entity.Id,
        PatientId = entity.PatientId,
        ProviderId = entity.ProviderId,
        PrimaryPatientInsuranceId = entity.PrimaryPatientInsuranceId,
        SecondaryPatientInsuranceId = entity.SecondaryPatientInsuranceId,
        DepartmentId = entity.DepartmentId,
        PatientDepartmentId = entity.PatientDepartmentId,
        Diagnosis1 = entity.Diagnosis1,
        Diagnosis2 = entity.Diagnosis2,
        Diagnosis3 = entity.Diagnosis3,
        Diagnosis4 = entity.Diagnosis4,
        Diagnosis5 = entity.Diagnosis5,
        Diagnosis6 = entity.Diagnosis6,
        Diagnosis7 = entity.Diagnosis7,
        Diagnosis8 = entity.Diagnosis8,
        ReferringProviderId = entity.ReferringProviderId,
        SupervisingProviderId = entity.SupervisingProviderId,
        AppointmentId = entity.AppointmentId,
        CurrentIllnessDate = entity.CurrentIllnessDate,
        ServiceDate = entity.ServiceDate,
        Status1 = entity.Status1,
        Status2 = entity.Status2,
        StatusP = entity.StatusP,
        Outstanding1 = entity.Outstanding1,
        Outstanding2 = entity.Outstanding2,
        OutstandingP = entity.OutstandingP,
        LastBilledDate1 = entity.LastBilledDate1,
        LastBilledDate2 = entity.LastBilledDate2,
        LastBilledDateP = entity.LastBilledDateP,
        HealthcareClaimTypeId1 = entity.HealthcareClaimTypeId1,
        HealthcareClaimTypeId2 = entity.HealthcareClaimTypeId2
    };

    public EfClaim ToEntity(ClaimItem item) => new()
    {
        Id = item.Id,
        PatientId = item.PatientId,
        ProviderId = item.ProviderId,
        PrimaryPatientInsuranceId = item.PrimaryPatientInsuranceId,
        SecondaryPatientInsuranceId = item.SecondaryPatientInsuranceId,
        DepartmentId = item.DepartmentId,
        PatientDepartmentId = item.PatientDepartmentId,
        Diagnosis1 = item.Diagnosis1,
        Diagnosis2 = item.Diagnosis2,
        Diagnosis3 = item.Diagnosis3,
        Diagnosis4 = item.Diagnosis4,
        Diagnosis5 = item.Diagnosis5,
        Diagnosis6 = item.Diagnosis6,
        Diagnosis7 = item.Diagnosis7,
        Diagnosis8 = item.Diagnosis8,
        ReferringProviderId = item.ReferringProviderId,
        SupervisingProviderId = item.SupervisingProviderId,
        AppointmentId = item.AppointmentId,
        CurrentIllnessDate = item.CurrentIllnessDate,
        ServiceDate = item.ServiceDate,
        Status1 = item.Status1,
        Status2 = item.Status2,
        StatusP = item.StatusP,
        Outstanding1 = item.Outstanding1,
        Outstanding2 = item.Outstanding2,
        OutstandingP = item.OutstandingP,
        LastBilledDate1 = item.LastBilledDate1,
        LastBilledDate2 = item.LastBilledDate2,
        LastBilledDateP = item.LastBilledDateP,
        HealthcareClaimTypeId1 = item.HealthcareClaimTypeId1,
        HealthcareClaimTypeId2 = item.HealthcareClaimTypeId2
    };
}
