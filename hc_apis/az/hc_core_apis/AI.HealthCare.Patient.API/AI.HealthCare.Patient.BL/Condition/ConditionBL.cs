using AI.HealthCare.Patient.Models.Condition;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class ConditionBL : IConditionBL
{
    private readonly IConditionRepository _conditionRepository;

    public ConditionBL(IConditionRepository conditionRepository)
    {
        _conditionRepository = conditionRepository;
    }

    public async Task<ConditionsModel> Create(ConditionsModel conditionsModel)
    {
        conditionsModel.ConditionItem = ToItem(conditionsModel.ConditionRequest);

        var savedItem = await _conditionRepository.Create(conditionsModel.ConditionItem);
        conditionsModel.ConditionItem = savedItem;

        conditionsModel.ConditionResponse = ToResponse(savedItem);
        conditionsModel.IsNotValid = false;
        conditionsModel.Message = "Condition created successfully.";

        return conditionsModel;
    }

    public async Task<ConditionsModel> GetById(ConditionsModel conditionsModel)
    {
        var item = await _conditionRepository.GetById(conditionsModel.ConditionItem.Id);
        if (item is null)
        {
            conditionsModel.IsNotValid = true;
            conditionsModel.Message = "Condition not found.";
            return conditionsModel;
        }

        conditionsModel.ConditionItem = item;
        conditionsModel.ConditionResponse = ToResponse(item);
        conditionsModel.IsNotValid = false;
        conditionsModel.Message = "Condition retrieved successfully.";
        return conditionsModel;
    }

    public async Task<ConditionsModel> GetAll(ConditionsModel conditionsModel)
    {
        var items = await _conditionRepository.GetAll();
        conditionsModel.ConditionItems = items;
        conditionsModel.IsNotValid = false;
        conditionsModel.Message = $"{items.Count} condition(s) retrieved.";
        return conditionsModel;
    }

    public async Task<ConditionsModel> GetByPatientId(Guid patientId)
    {
        var items = await _conditionRepository.GetByPatientId(patientId);
        return new ConditionsModel
        {
            ConditionItems = items,
            IsNotValid = false,
            Message = $"{items.Count} condition(s) retrieved for patient."
        };
    }

    public async Task<ConditionsModel> Update(ConditionsModel conditionsModel)
    {
        var existing = await _conditionRepository.GetById(conditionsModel.ConditionItem.Id);
        if (existing is null)
        {
            conditionsModel.IsNotValid = true;
            conditionsModel.Message = "Condition not found.";
            return conditionsModel;
        }

        var updatedItem = ToItem(conditionsModel.ConditionRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _conditionRepository.Update(updatedItem);
        conditionsModel.ConditionItem = savedItem!;
        conditionsModel.ConditionResponse = ToResponse(savedItem!);
        conditionsModel.IsNotValid = false;
        conditionsModel.Message = "Condition updated successfully.";
        return conditionsModel;
    }

    public async Task<ConditionsModel> Delete(ConditionsModel conditionsModel)
    {
        var deleted = await _conditionRepository.Delete(conditionsModel.ConditionItem.Id);
        if (!deleted)
        {
            conditionsModel.IsNotValid = true;
            conditionsModel.Message = "Condition not found.";
            return conditionsModel;
        }

        conditionsModel.IsNotValid = false;
        conditionsModel.Message = "Condition deleted successfully.";
        return conditionsModel;
    }

    private static ConditionItem ToItem(ConditionRequest request) => new()
    {
        Start = request.Start,
        Stop = request.Stop,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        System = request.System,
        Code = request.Code,
        Description = request.Description
    };

    private static ConditionResponse ToResponse(ConditionItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        System = item.System,
        Code = item.Code,
        Description = item.Description
    };
}
