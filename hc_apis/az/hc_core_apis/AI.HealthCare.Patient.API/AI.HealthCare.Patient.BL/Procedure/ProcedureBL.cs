using AI.HealthCare.Patient.Models.Procedure;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class ProcedureBL : IProcedureBL
{
    private readonly IProcedureRepository _procedureRepository;

    public ProcedureBL(IProcedureRepository procedureRepository)
    {
        _procedureRepository = procedureRepository;
    }

    public async Task<ProceduresModel> Create(ProceduresModel proceduresModel)
    {
        proceduresModel.ProcedureItem = ToItem(proceduresModel.ProcedureRequest);

        var savedItem = await _procedureRepository.Create(proceduresModel.ProcedureItem);
        proceduresModel.ProcedureItem = savedItem;

        proceduresModel.ProcedureResponse = ToResponse(savedItem);
        proceduresModel.IsNotValid = false;
        proceduresModel.Message = "Procedure created successfully.";

        return proceduresModel;
    }

    public async Task<ProceduresModel> GetById(ProceduresModel proceduresModel)
    {
        var item = await _procedureRepository.GetById(proceduresModel.ProcedureItem.Id);
        if (item is null)
        {
            proceduresModel.IsNotValid = true;
            proceduresModel.Message = "Procedure not found.";
            return proceduresModel;
        }

        proceduresModel.ProcedureItem = item;
        proceduresModel.ProcedureResponse = ToResponse(item);
        proceduresModel.IsNotValid = false;
        proceduresModel.Message = "Procedure retrieved successfully.";
        return proceduresModel;
    }

    public async Task<ProceduresModel> GetAll(ProceduresModel proceduresModel)
    {
        var items = await _procedureRepository.GetAll();
        proceduresModel.ProcedureItems = items;
        proceduresModel.IsNotValid = false;
        proceduresModel.Message = $"{items.Count} procedure(s) retrieved.";
        return proceduresModel;
    }

    public async Task<ProceduresModel> GetByPatientId(Guid patientId)
    {
        var items = await _procedureRepository.GetByPatientId(patientId);
        return new ProceduresModel
        {
            ProcedureItems = items,
            IsNotValid = false,
            Message = $"{items.Count} procedure(s) retrieved for patient."
        };
    }

    public async Task<ProceduresModel> Update(ProceduresModel proceduresModel)
    {
        var existing = await _procedureRepository.GetById(proceduresModel.ProcedureItem.Id);
        if (existing is null)
        {
            proceduresModel.IsNotValid = true;
            proceduresModel.Message = "Procedure not found.";
            return proceduresModel;
        }

        var updatedItem = ToItem(proceduresModel.ProcedureRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _procedureRepository.Update(updatedItem);
        proceduresModel.ProcedureItem = savedItem!;
        proceduresModel.ProcedureResponse = ToResponse(savedItem!);
        proceduresModel.IsNotValid = false;
        proceduresModel.Message = "Procedure updated successfully.";
        return proceduresModel;
    }

    public async Task<ProceduresModel> Delete(ProceduresModel proceduresModel)
    {
        var deleted = await _procedureRepository.Delete(proceduresModel.ProcedureItem.Id);
        if (!deleted)
        {
            proceduresModel.IsNotValid = true;
            proceduresModel.Message = "Procedure not found.";
            return proceduresModel;
        }

        proceduresModel.IsNotValid = false;
        proceduresModel.Message = "Procedure deleted successfully.";
        return proceduresModel;
    }

    private static ProcedureItem ToItem(ProcedureRequest request) => new()
    {
        Start = request.Start,
        Stop = request.Stop,
        PatientId = request.PatientId,
        EncounterId = request.EncounterId,
        System = request.System,
        Code = request.Code,
        Description = request.Description,
        BaseCost = request.BaseCost,
        ReasonCode = request.ReasonCode,
        ReasonDescription = request.ReasonDescription
    };

    private static ProcedureResponse ToResponse(ProcedureItem item) => new()
    {
        Id = item.Id,
        Start = item.Start,
        Stop = item.Stop,
        PatientId = item.PatientId,
        EncounterId = item.EncounterId,
        System = item.System,
        Code = item.Code,
        Description = item.Description,
        BaseCost = item.BaseCost,
        ReasonCode = item.ReasonCode,
        ReasonDescription = item.ReasonDescription
    };
}
