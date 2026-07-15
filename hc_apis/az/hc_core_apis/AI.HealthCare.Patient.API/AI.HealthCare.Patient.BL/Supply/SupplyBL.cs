using AI.HealthCare.Patient.Models.Supply;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class SupplyBL : ISupplyBL
{
    private readonly ISupplyRepository _supplyRepository;

    public SupplyBL(ISupplyRepository supplyRepository)
    {
        _supplyRepository = supplyRepository;
    }

    public async Task<SuppliesModel> Create(SuppliesModel suppliesModel)
    {
        suppliesModel.SupplyItem = ToItem(suppliesModel.SupplyRequest);

        var savedItem = await _supplyRepository.Create(suppliesModel.SupplyItem);
        suppliesModel.SupplyItem = savedItem;

        suppliesModel.SupplyResponse = ToResponse(savedItem);
        suppliesModel.IsNotValid = false;
        suppliesModel.Message = "Supply created successfully.";

        return suppliesModel;
    }

    public async Task<SuppliesModel> GetById(SuppliesModel suppliesModel)
    {
        var item = await _supplyRepository.GetById(suppliesModel.SupplyItem.Id);
        if (item is null)
        {
            suppliesModel.IsNotValid = true;
            suppliesModel.Message = "Supply not found.";
            return suppliesModel;
        }

        suppliesModel.SupplyItem = item;
        suppliesModel.SupplyResponse = ToResponse(item);
        suppliesModel.IsNotValid = false;
        suppliesModel.Message = "Supply retrieved successfully.";
        return suppliesModel;
    }

    public async Task<SuppliesModel> GetAll(SuppliesModel suppliesModel)
    {
        var items = await _supplyRepository.GetAll();
        suppliesModel.SupplyItems = items;
        suppliesModel.IsNotValid = false;
        suppliesModel.Message = $"{items.Count} supply(ies) retrieved.";
        return suppliesModel;
    }

    public async Task<SuppliesModel> GetByPatientId(Guid patientId)
    {
        var items = await _supplyRepository.GetByPatientId(patientId);
        return new SuppliesModel
        {
            SupplyItems = items,
            IsNotValid = false,
            Message = $"{items.Count} supply(ies) retrieved for patient."
        };
    }

    public async Task<SuppliesModel> Update(SuppliesModel suppliesModel)
    {
        var existing = await _supplyRepository.GetById(suppliesModel.SupplyItem.Id);
        if (existing is null)
        {
            suppliesModel.IsNotValid = true;
            suppliesModel.Message = "Supply not found.";
            return suppliesModel;
        }

        var updatedItem = ToItem(suppliesModel.SupplyRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _supplyRepository.Update(updatedItem);
        suppliesModel.SupplyItem = savedItem!;
        suppliesModel.SupplyResponse = ToResponse(savedItem!);
        suppliesModel.IsNotValid = false;
        suppliesModel.Message = "Supply updated successfully.";
        return suppliesModel;
    }

    public async Task<SuppliesModel> Delete(SuppliesModel suppliesModel)
    {
        var deleted = await _supplyRepository.Delete(suppliesModel.SupplyItem.Id);
        if (!deleted)
        {
            suppliesModel.IsNotValid = true;
            suppliesModel.Message = "Supply not found.";
            return suppliesModel;
        }

        suppliesModel.IsNotValid = false;
        suppliesModel.Message = "Supply deleted successfully.";
        return suppliesModel;
    }

    private static SupplyItem ToItem(SupplyRequest request) => new()
    {
        Date = request.Date,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        Code = request.Code,
        Description = request.Description,
        Quantity = request.Quantity
    };

    private static SupplyResponse ToResponse(SupplyItem item) => new()
    {
        Id = item.Id,
        Date = item.Date,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        Code = item.Code,
        Description = item.Description,
        Quantity = item.Quantity
    };
}
