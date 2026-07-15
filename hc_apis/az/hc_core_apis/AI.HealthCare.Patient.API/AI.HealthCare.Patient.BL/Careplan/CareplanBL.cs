using AI.HealthCare.Patient.Models.Careplan;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class CareplanBL : ICareplanBL
{
    private readonly ICareplanRepository _careplanRepository;

    public CareplanBL(ICareplanRepository careplanRepository)
    {
        _careplanRepository = careplanRepository;
    }

    public async Task<CareplansModel> Create(CareplansModel careplansModel)
    {
        careplansModel.CareplanItem = ToItem(careplansModel.CareplanRequest);
        careplansModel.CareplanItem.Id = Guid.NewGuid();

        var savedItem = await _careplanRepository.Create(careplansModel.CareplanItem);
        careplansModel.CareplanItem = savedItem;

        careplansModel.CareplanResponse = ToResponse(savedItem);
        careplansModel.IsNotValid = false;
        careplansModel.Message = "Careplan created successfully.";

        return careplansModel;
    }

    public async Task<CareplansModel> GetById(CareplansModel careplansModel)
    {
        var item = await _careplanRepository.GetById(careplansModel.CareplanItem.Id);
        if (item is null)
        {
            careplansModel.IsNotValid = true;
            careplansModel.Message = "Careplan not found.";
            return careplansModel;
        }

        careplansModel.CareplanItem = item;
        careplansModel.CareplanResponse = ToResponse(item);
        careplansModel.IsNotValid = false;
        careplansModel.Message = "Careplan retrieved successfully.";
        return careplansModel;
    }

    public async Task<CareplansModel> GetAll(CareplansModel careplansModel)
    {
        var items = await _careplanRepository.GetAll();
        careplansModel.CareplanItems = items;
        careplansModel.IsNotValid = false;
        careplansModel.Message = $"{items.Count} careplan(s) retrieved.";
        return careplansModel;
    }

    public async Task<CareplansModel> GetByPatientId(Guid patientId)
    {
        var items = await _careplanRepository.GetByPatientId(patientId);
        return new CareplansModel
        {
            CareplanItems = items,
            IsNotValid = false,
            Message = $"{items.Count} careplan(s) retrieved for patient."
        };
    }

    public async Task<CareplansModel> Update(CareplansModel careplansModel)
    {
        var existing = await _careplanRepository.GetById(careplansModel.CareplanItem.Id);
        if (existing is null)
        {
            careplansModel.IsNotValid = true;
            careplansModel.Message = "Careplan not found.";
            return careplansModel;
        }

        var updatedItem = ToItem(careplansModel.CareplanRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _careplanRepository.Update(updatedItem);
        careplansModel.CareplanItem = savedItem!;
        careplansModel.CareplanResponse = ToResponse(savedItem!);
        careplansModel.IsNotValid = false;
        careplansModel.Message = "Careplan updated successfully.";
        return careplansModel;
    }

    public async Task<CareplansModel> Delete(CareplansModel careplansModel)
    {
        var deleted = await _careplanRepository.Delete(careplansModel.CareplanItem.Id);
        if (!deleted)
        {
            careplansModel.IsNotValid = true;
            careplansModel.Message = "Careplan not found.";
            return careplansModel;
        }

        careplansModel.IsNotValid = false;
        careplansModel.Message = "Careplan deleted successfully.";
        return careplansModel;
    }

    private static CareplanItem ToItem(CareplanRequest request) => new()
    {
        Start = request.Start,
        Stop = request.Stop,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        Code = request.Code,
        Description = request.Description,
        ReasonCode = request.ReasonCode,
        ReasonDescription = request.ReasonDescription
    };

    private static CareplanResponse ToResponse(CareplanItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        Code = item.Code,
        Description = item.Description,
        ReasonCode = item.ReasonCode,
        ReasonDescription = item.ReasonDescription
    };
}
