using AI.HealthCare.Patient.Models.Claim;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class ClaimBL : IClaimBL
{
    private readonly IClaimRepository _claimRepository;

    public ClaimBL(IClaimRepository claimRepository)
    {
        _claimRepository = claimRepository;
    }

    public async Task<ClaimsModel> Create(ClaimsModel claimsModel)
    {
        claimsModel.ClaimItem = ToItem(claimsModel.ClaimRequest);
        claimsModel.ClaimItem.Id = Guid.NewGuid();

        var savedItem = await _claimRepository.Create(claimsModel.ClaimItem);
        claimsModel.ClaimItem = savedItem;

        claimsModel.ClaimResponse = ToResponse(savedItem);
        claimsModel.IsNotValid = false;
        claimsModel.Message = "Claim created successfully.";

        return claimsModel;
    }

    public async Task<ClaimsModel> GetById(ClaimsModel claimsModel)
    {
        var item = await _claimRepository.GetById(claimsModel.ClaimItem.Id);
        if (item is null)
        {
            claimsModel.IsNotValid = true;
            claimsModel.Message = "Claim not found.";
            return claimsModel;
        }

        claimsModel.ClaimItem = item;
        claimsModel.ClaimResponse = ToResponse(item);
        claimsModel.IsNotValid = false;
        claimsModel.Message = "Claim retrieved successfully.";
        return claimsModel;
    }

    public async Task<ClaimsModel> GetAll(ClaimsModel claimsModel)
    {
        var items = await _claimRepository.GetAll();
        claimsModel.ClaimItems = items;
        claimsModel.IsNotValid = false;
        claimsModel.Message = $"{items.Count} claim(s) retrieved.";
        return claimsModel;
    }

    public async Task<ClaimsModel> GetByPatientId(Guid patientId)
    {
        var items = await _claimRepository.GetByPatientId(patientId);
        return new ClaimsModel
        {
            ClaimItems = items,
            IsNotValid = false,
            Message = $"{items.Count} claim(s) retrieved for patient."
        };
    }

    public async Task<ClaimsModel> Update(ClaimsModel claimsModel)
    {
        var existing = await _claimRepository.GetById(claimsModel.ClaimItem.Id);
        if (existing is null)
        {
            claimsModel.IsNotValid = true;
            claimsModel.Message = "Claim not found.";
            return claimsModel;
        }

        var updatedItem = ToItem(claimsModel.ClaimRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _claimRepository.Update(updatedItem);
        claimsModel.ClaimItem = savedItem!;
        claimsModel.ClaimResponse = ToResponse(savedItem!);
        claimsModel.IsNotValid = false;
        claimsModel.Message = "Claim updated successfully.";
        return claimsModel;
    }

    public async Task<ClaimsModel> Delete(ClaimsModel claimsModel)
    {
        var deleted = await _claimRepository.Delete(claimsModel.ClaimItem.Id);
        if (!deleted)
        {
            claimsModel.IsNotValid = true;
            claimsModel.Message = "Claim not found.";
            return claimsModel;
        }

        claimsModel.IsNotValid = false;
        claimsModel.Message = "Claim deleted successfully.";
        return claimsModel;
    }

    private static ClaimItem ToItem(ClaimRequest request) => new()
    {
        PatientId = request.PatientId,
        ProviderId = request.ProviderId,
        PrimaryPatientInsuranceId = request.PrimaryPatientInsuranceId,
        SecondaryPatientInsuranceId = request.SecondaryPatientInsuranceId,
        DepartmentId = request.DepartmentId,
        PatientDepartmentId = request.PatientDepartmentId,
        Diagnosis1 = request.Diagnosis1,
        Diagnosis2 = request.Diagnosis2,
        Diagnosis3 = request.Diagnosis3,
        Diagnosis4 = request.Diagnosis4,
        Diagnosis5 = request.Diagnosis5,
        Diagnosis6 = request.Diagnosis6,
        Diagnosis7 = request.Diagnosis7,
        Diagnosis8 = request.Diagnosis8,
        ReferringProviderId = request.ReferringProviderId,
        SupervisingProviderId = request.SupervisingProviderId,
        AppointmentId = request.AppointmentId,
        CurrentIllnessDate = request.CurrentIllnessDate,
        ServiceDate = request.ServiceDate,
        Status1 = request.Status1,
        Status2 = request.Status2,
        StatusP = request.StatusP,
        Outstanding1 = request.Outstanding1,
        Outstanding2 = request.Outstanding2,
        OutstandingP = request.OutstandingP,
        LastBilledDate1 = request.LastBilledDate1,
        LastBilledDate2 = request.LastBilledDate2,
        LastBilledDateP = request.LastBilledDateP,
        HealthcareClaimTypeId1 = request.HealthcareClaimTypeId1,
        HealthcareClaimTypeId2 = request.HealthcareClaimTypeId2
    };

    private static ClaimResponse ToResponse(ClaimItem item) => new()
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
