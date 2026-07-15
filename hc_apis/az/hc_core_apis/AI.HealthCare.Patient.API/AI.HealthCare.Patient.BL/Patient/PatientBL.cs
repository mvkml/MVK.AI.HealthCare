using AI.HealthCare.Patient.Models.Patient;
using AI.HealthCare.Patient.Models.Shared;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public class PatientBL : IPatientBL
{
    private const int ImportBatchSize = 500;

    private readonly IPatientRepository _patientRepository;
    private readonly IPatientBLMapper _mapper;

    public PatientBL(IPatientRepository patientRepository, IPatientBLMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<PatientsModel> Create(PatientsModel patientsModel)
    {
        patientsModel.PatientItem = _mapper.ToItem(patientsModel.PatientRequest);
        patientsModel.PatientItem.Id = Guid.NewGuid();

        var savedItem = await _patientRepository.Create(patientsModel.PatientItem);
        patientsModel.PatientItem = savedItem;

        patientsModel.PatientResponse = _mapper.ToResponse(savedItem, patientsModel.IncludePii);
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
        patientsModel.PatientResponse = _mapper.ToResponse(item, patientsModel.IncludePii);
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

        var updatedItem = _mapper.ToItem(patientsModel.PatientRequest);
        updatedItem.Id = existing.Id;

        var savedItem = await _patientRepository.Update(updatedItem);
        patientsModel.PatientItem = savedItem!;
        patientsModel.PatientResponse = _mapper.ToResponse(savedItem!, patientsModel.IncludePii);
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

    public async Task<ImportResult> Import(Stream csvStream) =>
        await RunImport(csvStream, FlushBatch);

    public async Task<ImportResult> ImportUpsert(Stream csvStream) =>
        await RunImport(csvStream, FlushUpsertBatch);

    private async Task<ImportResult> RunImport(Stream csvStream, Func<List<PatientItem>, ImportResult, Task> flush)
    {
        var result = new ImportResult();
        var batch = new List<PatientItem>();

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
                await flush(batch, result);
        }

        if (batch.Count > 0)
            await flush(batch, result);

        return result;
    }

    private async Task FlushBatch(List<PatientItem> batch, ImportResult result)
    {
        try
        {
            await _patientRepository.CreateBatch(batch);
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

    private async Task FlushUpsertBatch(List<PatientItem> batch, ImportResult result)
    {
        try
        {
            await _patientRepository.UpsertBatch(batch);
            result.InsertedCount += batch.Count;
        }
        catch (Exception ex)
        {
            result.FailedCount += batch.Count;
            result.Errors.Add(new ImportRowError { RowNumber = -1, ErrorMessage = $"Batch upsert failed: {ex.Message}" });
        }
        finally
        {
            batch.Clear();
        }
    }
}
