using System.Globalization;
using AI.HealthCare.Patient.Models.Payer;

namespace AI.HealthCare.Patient.BL;

// Maps a row from Synthea's payers.csv:
// Id,NAME,OWNERSHIP,ADDRESS,CITY,STATE_HEADQUARTERED,ZIP,PHONE,AMOUNT_COVERED,AMOUNT_UNCOVERED,
// REVENUE,COVERED_ENCOUNTERS,UNCOVERED_ENCOUNTERS,COVERED_MEDICATIONS,UNCOVERED_MEDICATIONS,
// COVERED_PROCEDURES,UNCOVERED_PROCEDURES,COVERED_IMMUNIZATIONS,UNCOVERED_IMMUNIZATIONS,
// UNIQUE_CUSTOMERS,QOLS_AVG,MEMBER_MONTHS
public class PayerBLMapper : IPayerBLMapper
{
    private const int ExpectedColumnCount = 22;

    public PayerItem ToModel(string[] row)
    {
        if (row.Length < ExpectedColumnCount)
            throw new FormatException($"Expected {ExpectedColumnCount} columns, got {row.Length}.");

        return new PayerItem
        {
            Id = Guid.Parse(row[0]),
            Name = row[1],
            Ownership = NullIfEmpty(row[2]),
            Address = NullIfEmpty(row[3]),
            City = NullIfEmpty(row[4]),
            StateHeadquartered = NullIfEmpty(row[5]),
            Zip = NullIfEmpty(row[6]),
            Phone = NullIfEmpty(row[7]),
            AmountCovered = ParseDecimal(row[8]),
            AmountUncovered = ParseDecimal(row[9]),
            Revenue = ParseDecimal(row[10]),
            CoveredEncounters = ParseInt(row[11]),
            UncoveredEncounters = ParseInt(row[12]),
            CoveredMedications = ParseInt(row[13]),
            UncoveredMedications = ParseInt(row[14]),
            CoveredProcedures = ParseInt(row[15]),
            UncoveredProcedures = ParseInt(row[16]),
            CoveredImmunizations = ParseInt(row[17]),
            UncoveredImmunizations = ParseInt(row[18]),
            UniqueCustomers = ParseInt(row[19]),
            QolsAvg = ParseDecimal(row[20]),
            MemberMonths = ParseInt(row[21]),
        };
    }

    public PayerItem ToItem(PayerRequest request) => new()
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

    public PayerResponse ToResponse(PayerItem item) => new()
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

    private static decimal? ParseDecimal(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : decimal.Parse(value, CultureInfo.InvariantCulture);

    private static int? ParseInt(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : int.Parse(value, CultureInfo.InvariantCulture);

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
