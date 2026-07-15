using AI.HealthCare.Patient.Models.Allergy;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class AllergyBL : IAllergyBL
{
    private readonly IAllergyRepository _allergyRepository;

    public AllergyBL(IAllergyRepository allergyRepository)
    {
        _allergyRepository = allergyRepository;
    }

    public async Task<AllergiesModel> Create(AllergiesModel allergiesModel)
    {
        allergiesModel.AllergyItem = ToItem(allergiesModel.AllergyRequest);

        var savedItem = await _allergyRepository.Create(allergiesModel.AllergyItem);
        allergiesModel.AllergyItem = savedItem;

        allergiesModel.AllergyResponse = ToResponse(savedItem);
        allergiesModel.IsNotValid = false;
        allergiesModel.Message = "Allergy created successfully.";

        return allergiesModel;
    }

    public async Task<AllergiesModel> GetById(AllergiesModel allergiesModel)
    {
        var item = await _allergyRepository.GetById(allergiesModel.AllergyItem.Id);
        if (item is null)
        {
            allergiesModel.IsNotValid = true;
            allergiesModel.Message = "Allergy not found.";
            return allergiesModel;
        }

        allergiesModel.AllergyItem = item;
        allergiesModel.AllergyResponse = ToResponse(item);
        allergiesModel.IsNotValid = false;
        allergiesModel.Message = "Allergy retrieved successfully.";
        return allergiesModel;
    }

    public async Task<AllergiesModel> GetAll(AllergiesModel allergiesModel)
    {
        var items = await _allergyRepository.GetAll();
        allergiesModel.AllergyItems = items;
        allergiesModel.IsNotValid = false;
        allergiesModel.Message = $"{items.Count} allergy(ies) retrieved.";
        return allergiesModel;
    }

    public async Task<AllergiesModel> GetByPatientId(Guid patientId)
    {
        var items = await _allergyRepository.GetByPatientId(patientId);
        return new AllergiesModel
        {
            AllergyItems = items,
            IsNotValid = false,
            Message = $"{items.Count} allergy(ies) retrieved for patient."
        };
    }

    public async Task<AllergiesModel> Update(AllergiesModel allergiesModel)
    {
        var existing = await _allergyRepository.GetById(allergiesModel.AllergyItem.Id);
        if (existing is null)
        {
            allergiesModel.IsNotValid = true;
            allergiesModel.Message = "Allergy not found.";
            return allergiesModel;
        }

        var updatedItem = ToItem(allergiesModel.AllergyRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _allergyRepository.Update(updatedItem);
        allergiesModel.AllergyItem = savedItem!;
        allergiesModel.AllergyResponse = ToResponse(savedItem!);
        allergiesModel.IsNotValid = false;
        allergiesModel.Message = "Allergy updated successfully.";
        return allergiesModel;
    }

    public async Task<AllergiesModel> Delete(AllergiesModel allergiesModel)
    {
        var deleted = await _allergyRepository.Delete(allergiesModel.AllergyItem.Id);
        if (!deleted)
        {
            allergiesModel.IsNotValid = true;
            allergiesModel.Message = "Allergy not found.";
            return allergiesModel;
        }

        allergiesModel.IsNotValid = false;
        allergiesModel.Message = "Allergy deleted successfully.";
        return allergiesModel;
    }

    private static AllergyItem ToItem(AllergyRequest request) => new()
    {
        Start = request.Start,
        Stop = request.Stop,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        Code = request.Code,
        System = request.System,
        Description = request.Description,
        Type = request.Type,
        Category = request.Category,
        Reaction1 = request.Reaction1,
        Description1 = request.Description1,
        Severity1 = request.Severity1,
        Reaction2 = request.Reaction2,
        Description2 = request.Description2,
        Severity2 = request.Severity2
    };

    private static AllergyResponse ToResponse(AllergyItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        Code = item.Code,
        System = item.System,
        Description = item.Description,
        Type = item.Type,
        Category = item.Category,
        Reaction1 = item.Reaction1,
        Description1 = item.Description1,
        Severity1 = item.Severity1,
        Reaction2 = item.Reaction2,
        Description2 = item.Description2,
        Severity2 = item.Severity2
    };
}
