using AI.HealthCare.Patient.Models.Observation;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class ObservationBL : IObservationBL
{
    private const int ImportBatchSize = 500;

    private readonly IObservationRepository _observationRepository;
    private readonly IObservationBLMapper _mapper;

    public ObservationBL(IObservationRepository observationRepository, IObservationBLMapper mapper)
    {
        _observationRepository = observationRepository;
        _mapper = mapper;
    }

    public async Task<ObservationsModel> Create(ObservationsModel observationsModel)
    {
        observationsModel.ObservationItem = _mapper.ToItem(observationsModel.ObservationRequest);

        var savedItem = await _observationRepository.Create(observationsModel.ObservationItem);
        observationsModel.ObservationItem = savedItem;

        observationsModel.ObservationResponse = _mapper.ToResponse(savedItem);
        observationsModel.IsNotValid = false;
        observationsModel.Message = "Observation created successfully.";

        return observationsModel;
    }

    public async Task<ObservationsModel> GetById(ObservationsModel observationsModel)
    {
        var item = await _observationRepository.GetById(observationsModel.ObservationItem.Id);
        if (item is null)
        {
            observationsModel.IsNotValid = true;
            observationsModel.Message = "Observation not found.";
            return observationsModel;
        }

        observationsModel.ObservationItem = item;
        observationsModel.ObservationResponse = _mapper.ToResponse(item);
        observationsModel.IsNotValid = false;
        observationsModel.Message = "Observation retrieved successfully.";
        return observationsModel;
    }

    public async Task<ObservationsModel> GetAll(ObservationsModel observationsModel)
    {
        var items = await _observationRepository.GetAll();
        observationsModel.ObservationItems = items;
        observationsModel.IsNotValid = false;
        observationsModel.Message = $"{items.Count} observation(s) retrieved.";
        return observationsModel;
    }

    public async Task<ObservationsModel> GetByPatientId(Guid patientId)
    {
        var items = await _observationRepository.GetByPatientId(patientId);
        return new ObservationsModel
        {
            ObservationItems = items,
            IsNotValid = false,
            Message = $"{items.Count} observation(s) retrieved for patient."
        };
    }

    public async Task<ObservationsModel> Update(ObservationsModel observationsModel)
    {
        var existing = await _observationRepository.GetById(observationsModel.ObservationItem.Id);
        if (existing is null)
        {
            observationsModel.IsNotValid = true;
            observationsModel.Message = "Observation not found.";
            return observationsModel;
        }

        var updatedItem = _mapper.ToItem(observationsModel.ObservationRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _observationRepository.Update(updatedItem);
        observationsModel.ObservationItem = savedItem!;
        observationsModel.ObservationResponse = _mapper.ToResponse(savedItem!);
        observationsModel.IsNotValid = false;
        observationsModel.Message = "Observation updated successfully.";
        return observationsModel;
    }

    public async Task<ObservationsModel> Delete(ObservationsModel observationsModel)
    {
        var deleted = await _observationRepository.Delete(observationsModel.ObservationItem.Id);
        if (!deleted)
        {
            observationsModel.IsNotValid = true;
            observationsModel.Message = "Observation not found.";
            return observationsModel;
        }

        observationsModel.IsNotValid = false;
        observationsModel.Message = "Observation deleted successfully.";
        return observationsModel;
    }

    public async Task<ImportResult> Import(Stream csvStream)
    {
        var result = new ImportResult();
        var batch = new List<ObservationItem>();

        using var reader = new StreamReader(csvStream);
        await reader.ReadLineAsync(); // skip header row

        var rowNumber = 1;
        while (!reader.EndOfStream)
        {
            rowNumber++;
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            result.TotalRows++;

            try
            {
                batch.Add(_mapper.ToModel(line.Split(',')));
            }
            catch (Exception ex)
            {
                result.FailedCount++;
                result.Errors.Add(new ImportRowError { RowNumber = rowNumber, ErrorMessage = ex.Message });
                continue;
            }

            if (batch.Count >= ImportBatchSize)
                await FlushBatch(batch, result);
        }

        if (batch.Count > 0)
            await FlushBatch(batch, result);

        return result;
    }

    private async Task FlushBatch(List<ObservationItem> batch, ImportResult result)
    {
        try
        {
            await _observationRepository.CreateBatch(batch);
            result.InsertedCount += batch.Count;
        }
        catch (Exception ex)
        {
            result.FailedCount += batch.Count;
            result.Errors.Add(new ImportRowError { RowNumber = -1, ErrorMessage = $"Batch insert failed: {ex.Message}" });
        }
        finally
        {
            batch.Clear();
        }
    }
}
