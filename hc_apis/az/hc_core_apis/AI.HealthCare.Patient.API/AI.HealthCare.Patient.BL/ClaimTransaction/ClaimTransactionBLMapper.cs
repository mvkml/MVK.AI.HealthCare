using System.Globalization;
using AI.HealthCare.Patient.Models.ClaimTransaction;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's claims_transactions.csv:
// ID,CLAIMID,CHARGEID,PATIENTID,TYPE,AMOUNT,METHOD,FROMDATE,TODATE,PLACEOFSERVICE,PROCEDURECODE,
// MODIFIER1,MODIFIER2,DIAGNOSISREF1,DIAGNOSISREF2,DIAGNOSISREF3,DIAGNOSISREF4,UNITS,DEPARTMENTID,
// NOTES,UNITAMOUNT,TRANSFEROUTID,TRANSFERTYPE,PAYMENTS,ADJUSTMENTS,TRANSFERS,OUTSTANDING,
// APPOINTMENTID,LINENOTE,PATIENTINSURANCEID,FEESCHEDULEID,PROVIDERID,SUPERVISINGPROVIDERID
public class ClaimTransactionBLMapper : IClaimTransactionBLMapper
{
    private const int ExpectedColumnCount = 33;

    public ClaimTransactionItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new ClaimTransactionItem
        {
            Id = Guid.Parse(row[0]),
            ClaimId = Guid.Parse(row[1]),
            ChargeId = int.Parse(row[2], CultureInfo.InvariantCulture),
            PatientId = Guid.Parse(row[3]),
            Type = row[4],
            Amount = ParseNullableDecimal(row[5]),
            Method = NullIfEmpty(row[6]),
            FromDate = DateTime.Parse(row[7], CultureInfo.InvariantCulture),
            ToDate = DateTime.Parse(row[8], CultureInfo.InvariantCulture),
            PlaceOfServiceId = ParseNullableGuid(row[9]),
            ProcedureCode = NullIfEmpty(row[10]),
            Modifier1 = NullIfEmpty(row[11]),
            Modifier2 = NullIfEmpty(row[12]),
            DiagnosisRef1 = ParseNullableInt(row[13]),
            DiagnosisRef2 = ParseNullableInt(row[14]),
            DiagnosisRef3 = ParseNullableInt(row[15]),
            DiagnosisRef4 = ParseNullableInt(row[16]),
            Units = int.Parse(row[17], CultureInfo.InvariantCulture),
            DepartmentId = ParseNullableInt(row[18]),
            Notes = NullIfEmpty(row[19]),
            UnitAmount = decimal.Parse(row[20], CultureInfo.InvariantCulture),
            TransferOutId = NullIfEmpty(row[21]),
            TransferType = NullIfEmpty(row[22]),
            Payments = ParseNullableDecimal(row[23]),
            Adjustments = ParseNullableDecimal(row[24]),
            Transfers = ParseNullableDecimal(row[25]),
            Outstanding = ParseNullableDecimal(row[26]),
            AppointmentId = ParseNullableGuid(row[27]),
            LineNote = NullIfEmpty(row[28]),
            PatientInsuranceId = ParseNullableGuid(row[29]),
            FeeScheduleId = ParseNullableInt(row[30]),
            ProviderId = Guid.Parse(row[31]),
            SupervisingProviderId = Guid.Parse(row[32]),
        };
    }

    public ClaimTransactionItem ToItem(ClaimTransactionRequest request) => new()
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

    public ClaimTransactionResponse ToResponse(ClaimTransactionItem item) => new()
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

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;

    private static Guid? ParseNullableGuid(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : Guid.Parse(value);

    private static int? ParseNullableInt(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : int.Parse(value, CultureInfo.InvariantCulture);

    private static decimal? ParseNullableDecimal(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : decimal.Parse(value, CultureInfo.InvariantCulture);
}
