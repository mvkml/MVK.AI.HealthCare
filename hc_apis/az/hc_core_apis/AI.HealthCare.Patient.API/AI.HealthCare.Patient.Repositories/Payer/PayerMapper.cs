using AI.HealthCare.Patient.Models.Payer;
using EfPayer = AI.HealthCare.Patient.EF.Entities.Payer;

namespace AI.HealthCare.Patient.Repositories;

public class PayerMapper : IPayerMapper
{
    public PayerItem ToModel(EfPayer entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Ownership = entity.Ownership,
        Address = entity.Address,
        City = entity.City,
        StateHeadquartered = entity.StateHeadquartered,
        Zip = entity.Zip,
        Phone = entity.Phone,
        AmountCovered = entity.AmountCovered,
        AmountUncovered = entity.AmountUncovered,
        Revenue = entity.Revenue,
        CoveredEncounters = entity.CoveredEncounters,
        UncoveredEncounters = entity.UncoveredEncounters,
        CoveredMedications = entity.CoveredMedications,
        UncoveredMedications = entity.UncoveredMedications,
        CoveredProcedures = entity.CoveredProcedures,
        UncoveredProcedures = entity.UncoveredProcedures,
        CoveredImmunizations = entity.CoveredImmunizations,
        UncoveredImmunizations = entity.UncoveredImmunizations,
        UniqueCustomers = entity.UniqueCustomers,
        QolsAvg = entity.QolsAvg,
        MemberMonths = entity.MemberMonths
    };

    public EfPayer ToEntity(PayerItem item) => new()
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
