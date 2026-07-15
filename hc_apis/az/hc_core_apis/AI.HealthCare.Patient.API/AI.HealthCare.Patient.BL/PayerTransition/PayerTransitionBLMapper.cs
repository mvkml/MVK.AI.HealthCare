using System.Globalization;
using AI.HealthCare.Patient.Models.PayerTransition;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's payer_transitions.csv:
// PATIENT,MEMBERID,START_DATE,END_DATE,PAYER,SECONDARY_PAYER,PLAN_OWNERSHIP,OWNER_NAME
public class PayerTransitionBLMapper : IPayerTransitionBLMapper
{
    private const int ExpectedColumnCount = 8;

    public PayerTransitionItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new PayerTransitionItem
        {
            PatientId = Guid.Parse(row[0]),
            MemberId = Guid.Parse(row[1]),
            StartDate = ParseDate(row[2]),
            EndDate = ParseDate(row[3]),
            PayerId = Guid.Parse(row[4]),
            SecondaryPayerId = ParseGuid(row[5]),
            PlanOwnership = NullIfEmpty(row[6]),
            OwnerName = NullIfEmpty(row[7]),
        };
    }

    public PayerTransitionItem ToItem(PayerTransitionRequest request) => new()
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

    public PayerTransitionResponse ToResponse(PayerTransitionItem item) => new()
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

    private static DateTime? ParseDate(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

    private static Guid? ParseGuid(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : Guid.Parse(value);

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
