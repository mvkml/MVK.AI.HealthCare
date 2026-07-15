using System.Globalization;
using AI.HealthCare.Patient.Models.Claim;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's claims.csv:
// Id,PATIENTID,PROVIDERID,PRIMARYPATIENTINSURANCEID,SECONDARYPATIENTINSURANCEID,DEPARTMENTID,PATIENTDEPARTMENTID,
// DIAGNOSIS1,DIAGNOSIS2,DIAGNOSIS3,DIAGNOSIS4,DIAGNOSIS5,DIAGNOSIS6,DIAGNOSIS7,DIAGNOSIS8,
// REFERRINGPROVIDERID,APPOINTMENTID,CURRENTILLNESSDATE,SERVICEDATE,SUPERVISINGPROVIDERID,
// STATUS1,STATUS2,STATUSP,OUTSTANDING1,OUTSTANDING2,OUTSTANDINGP,
// LASTBILLEDDATE1,LASTBILLEDDATE2,LASTBILLEDDATEP,HEALTHCARECLAIMTYPEID1,HEALTHCARECLAIMTYPEID2
public class ClaimBLMapper : IClaimBLMapper
{
    private const int ExpectedColumnCount = 31;

    public ClaimItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new ClaimItem
        {
            Id = Guid.Parse(row[0]),
            PatientId = Guid.Parse(row[1]),
            ProviderId = Guid.Parse(row[2]),
            PrimaryPatientInsuranceId = ParseGuid(row[3]),
            SecondaryPatientInsuranceId = ParseGuid(row[4]),
            DepartmentId = ParseInt(row[5]),
            PatientDepartmentId = ParseInt(row[6]),
            Diagnosis1 = NullIfEmpty(row[7]),
            Diagnosis2 = NullIfEmpty(row[8]),
            Diagnosis3 = NullIfEmpty(row[9]),
            Diagnosis4 = NullIfEmpty(row[10]),
            Diagnosis5 = NullIfEmpty(row[11]),
            Diagnosis6 = NullIfEmpty(row[12]),
            Diagnosis7 = NullIfEmpty(row[13]),
            Diagnosis8 = NullIfEmpty(row[14]),
            ReferringProviderId = ParseGuid(row[15]),
            AppointmentId = ParseGuid(row[16]),
            CurrentIllnessDate = ParseDate(row[17]),
            ServiceDate = ParseDate(row[18]),
            SupervisingProviderId = ParseGuid(row[19]),
            Status1 = NullIfEmpty(row[20]),
            Status2 = NullIfEmpty(row[21]),
            StatusP = NullIfEmpty(row[22]),
            Outstanding1 = ParseDecimal(row[23]),
            Outstanding2 = ParseDecimal(row[24]),
            OutstandingP = ParseDecimal(row[25]),
            LastBilledDate1 = ParseDate(row[26]),
            LastBilledDate2 = ParseDate(row[27]),
            LastBilledDateP = ParseDate(row[28]),
            HealthcareClaimTypeId1 = ParseInt(row[29]),
            HealthcareClaimTypeId2 = ParseInt(row[30]),
        };
    }

    public ClaimItem ToItem(ClaimRequest request) => new()
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

    public ClaimResponse ToResponse(ClaimItem item) => new()
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

    private static DateTime? ParseDate(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

    private static Guid? ParseGuid(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : Guid.Parse(value);

    private static int? ParseInt(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : int.Parse(value, CultureInfo.InvariantCulture);

    private static decimal? ParseDecimal(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : decimal.Parse(value, CultureInfo.InvariantCulture);

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
