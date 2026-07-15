using AI.HealthCare.Patient.Models.Patient;
using EfPatient = AI.HealthCare.Patient.EF.Entities.Patient;

namespace AI.HealthCare.Patient.Repositories;

public class PatientMapper : IPatientMapper
{
    public PatientItem ToModel(EfPatient entity) => new()
    {
        Id = entity.Id,
        BirthDate = entity.BirthDate,
        DeathDate = entity.DeathDate,
        Ssn = entity.Ssn,
        Drivers = entity.Drivers,
        Passport = entity.Passport,
        Prefix = entity.Prefix,
        First = entity.First,
        Middle = entity.Middle,
        Last = entity.Last,
        Suffix = entity.Suffix,
        Maiden = entity.Maiden,
        Marital = entity.Marital,
        Race = entity.Race,
        Ethnicity = entity.Ethnicity,
        Gender = entity.Gender,
        Birthplace = entity.Birthplace,
        Address = entity.Address,
        City = entity.City,
        State = entity.State,
        County = entity.County,
        Fips = entity.Fips,
        Zip = entity.Zip,
        Lat = entity.Lat,
        Lon = entity.Lon,
        HealthcareExpenses = entity.HealthcareExpenses,
        HealthcareCoverage = entity.HealthcareCoverage,
        Income = entity.Income
    };

    public EfPatient ToEntity(PatientItem item) => new()
    {
        Id = item.Id,
        BirthDate = item.BirthDate,
        DeathDate = item.DeathDate,
        Ssn = item.Ssn,
        Drivers = item.Drivers,
        Passport = item.Passport,
        Prefix = item.Prefix,
        First = item.First,
        Middle = item.Middle,
        Last = item.Last,
        Suffix = item.Suffix,
        Maiden = item.Maiden,
        Marital = item.Marital,
        Race = item.Race,
        Ethnicity = item.Ethnicity,
        Gender = item.Gender,
        Birthplace = item.Birthplace,
        Address = item.Address,
        City = item.City,
        State = item.State,
        County = item.County,
        Fips = item.Fips,
        Zip = item.Zip,
        Lat = item.Lat,
        Lon = item.Lon,
        HealthcareExpenses = item.HealthcareExpenses,
        HealthcareCoverage = item.HealthcareCoverage,
        Income = item.Income
    };
}
