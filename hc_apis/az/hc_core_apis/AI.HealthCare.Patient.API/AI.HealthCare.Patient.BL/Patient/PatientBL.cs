using AI.HealthCare.Patient.Models.Patient;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class PatientBL : IPatientBL
{
    private readonly IPatientRepository _patientRepository;

    public PatientBL(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientsModel> Create(PatientsModel patientsModel)
    {
        patientsModel.PatientItem = ToPatientItem(patientsModel.PatientRequest);
        patientsModel.PatientItem.Id = Guid.NewGuid();

        var savedItem = await _patientRepository.Create(patientsModel.PatientItem);
        patientsModel.PatientItem = savedItem;

        patientsModel.PatientResponse = ToResponse(savedItem, patientsModel.IncludePii);
        patientsModel.IsNotValid = false;
        patientsModel.Message = "Patient created successfully.";

        return patientsModel;
    }

    public async Task<PatientsModel> GetById(PatientsModel patientsModel)
    {
        var item = await _patientRepository.GetById(patientsModel.PatientItem.Id);
        if (item is null)
        {
            patientsModel.IsNotValid = true;
            patientsModel.Message = "Patient not found.";
            return patientsModel;
        }

        patientsModel.PatientItem = item;
        patientsModel.PatientResponse = ToResponse(item, patientsModel.IncludePii);
        patientsModel.IsNotValid = false;
        patientsModel.Message = "Patient retrieved successfully.";
        return patientsModel;
    }

    public async Task<PatientsModel> GetAll(PatientsModel patientsModel)
    {
        var items = await _patientRepository.GetAll();
        patientsModel.PatientItems = items;
        patientsModel.IsNotValid = false;
        patientsModel.Message = $"{items.Count} patient(s) retrieved.";
        return patientsModel;
    }

    public async Task<PatientsModel> Update(PatientsModel patientsModel)
    {
        var existing = await _patientRepository.GetById(patientsModel.PatientItem.Id);
        if (existing is null)
        {
            patientsModel.IsNotValid = true;
            patientsModel.Message = "Patient not found.";
            return patientsModel;
        }

        var updatedItem = ToPatientItem(patientsModel.PatientRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _patientRepository.Update(updatedItem);
        patientsModel.PatientItem = savedItem!;
        patientsModel.PatientResponse = ToResponse(savedItem!, patientsModel.IncludePii);
        patientsModel.IsNotValid = false;
        patientsModel.Message = "Patient updated successfully.";
        return patientsModel;
    }

    public async Task<PatientsModel> Delete(PatientsModel patientsModel)
    {
        var deleted = await _patientRepository.Delete(patientsModel.PatientItem.Id);
        if (!deleted)
        {
            patientsModel.IsNotValid = true;
            patientsModel.Message = "Patient not found.";
            return patientsModel;
        }

        patientsModel.IsNotValid = false;
        patientsModel.Message = "Patient deleted successfully.";
        return patientsModel;
    }

    private static PatientItem ToPatientItem(PatientRequest request) => new()
    {
        BirthDate = request.BirthDate,
        DeathDate = request.DeathDate,
        Ssn = request.Ssn,
        Drivers = request.Drivers,
        Passport = request.Passport,
        Prefix = request.Prefix,
        First = request.First,
        Middle = request.Middle,
        Last = request.Last,
        Suffix = request.Suffix,
        Maiden = request.Maiden,
        Marital = request.Marital,
        Race = request.Race,
        Ethnicity = request.Ethnicity,
        Gender = request.Gender,
        Birthplace = request.Birthplace,
        Address = request.Address,
        City = request.City,
        State = request.State,
        County = request.County,
        Fips = request.Fips,
        Zip = request.Zip,
        Lat = request.Lat,
        Lon = request.Lon,
        HealthcareExpenses = request.HealthcareExpenses,
        HealthcareCoverage = request.HealthcareCoverage,
        Income = request.Income
    };

    private static PatientResponse ToResponse(PatientItem item, bool includePii) => new()
    {
        Id = item.Id,
        Ssn = includePii ? Mask(item.Ssn) : null,
        Drivers = includePii ? Mask(item.Drivers) : null,
        Passport = includePii ? Mask(item.Passport) : null,
        BirthDate = item.BirthDate,
        DeathDate = item.DeathDate,
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

    /// <summary>Masks all but the last 4 characters, e.g. "999-83-4938" -> "***-**-4938". For demo/learning purposes only — not a substitute for real PII encryption at rest.</summary>
    private static string? Mask(string? value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        if (value.Length <= 4) return new string('*', value.Length);

        var visible = value[^4..];
        var maskedPrefix = value[..^4].Select(c => c == '-' ? '-' : '*');
        return new string(maskedPrefix.ToArray()) + visible;
    }
}
