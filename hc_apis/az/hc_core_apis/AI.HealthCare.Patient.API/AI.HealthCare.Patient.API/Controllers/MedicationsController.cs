using AI.HealthCare.Patient.API.Shared;
using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Medication;
using AI.HealthCare.Patient.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicationsController : ControllerBase
{
    private readonly IMedicationBL _medicationBL;
    private readonly IMedicationValidationService _medicationValidationService;
    private readonly ICsvFileValidator _csvFileValidator;

    public MedicationsController(IMedicationBL medicationBL, IMedicationValidationService medicationValidationService, ICsvFileValidator csvFileValidator)
    {
        _medicationBL = medicationBL;
        _medicationValidationService = medicationValidationService;
        _csvFileValidator = csvFileValidator;
    }

    /// <summary>Creates a new medication.</summary>
    [HttpPost]
    public async Task<ActionResult<MedicationResponse>> Create([FromBody] MedicationRequest medicationRequest)
    {
        var medicationsModel = new MedicationsModel { MedicationRequest = medicationRequest };

        medicationsModel = _medicationValidationService.Validate(medicationsModel);
        if (medicationsModel.IsNotValid)
        {
            return BadRequest(new MedicationResponse
            {
                IsNotValid = true,
                Message = medicationsModel.Message,
            });
        }

        medicationsModel = await _medicationBL.Create(medicationsModel);
        return Ok(medicationsModel.MedicationResponse);
    }

    /// <summary>Returns all medications.</summary>
    [HttpGet]
    public async Task<ActionResult<List<MedicationItem>>> GetAll()
    {
        var result = await _medicationBL.GetAll(new MedicationsModel());
        return Ok(result.MedicationItems);
    }

    /// <summary>Returns all medications for a given patient.</summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<List<MedicationItem>>> GetByPatientId(Guid patientId)
    {
        var result = await _medicationBL.GetByPatientId(patientId);
        return Ok(result.MedicationItems);
    }

    /// <summary>Returns a single medication by Id.</summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<MedicationResponse>> GetById(long id)
    {
        var medicationsModel = new MedicationsModel { MedicationItem = new MedicationItem { Id = id } };
        var result = await _medicationBL.GetById(medicationsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.MedicationResponse);
    }

    /// <summary>Updates an existing medication.</summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<MedicationResponse>> Update(long id, [FromBody] MedicationRequest medicationRequest)
    {
        var medicationsModel = new MedicationsModel
        {
            MedicationRequest = medicationRequest,
            MedicationItem = new MedicationItem { Id = id }
        };

        medicationsModel = _medicationValidationService.Validate(medicationsModel);
        if (medicationsModel.IsNotValid)
        {
            return BadRequest(new MedicationResponse
            {
                IsNotValid = true,
                Message = medicationsModel.Message,
            });
        }

        medicationsModel = await _medicationBL.Update(medicationsModel);
        if (medicationsModel.IsNotValid)
            return NotFound(medicationsModel.Message);

        return Ok(medicationsModel.MedicationResponse);
    }

    /// <summary>Bulk imports medications from a CSV file (Synthea medications.csv format). Matching Patient, Payer, and Encounter records must already exist.</summary>
    [HttpPost("import")]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ImportResult>> Import(IFormFile file)
    {
        var (isValid, errorMessage) = _csvFileValidator.Validate(file);
        if (!isValid)
            return BadRequest(errorMessage);

        var result = await _medicationBL.Import(file.OpenReadStream());
        return Ok(result);
    }

    /// <summary>Deletes a medication by Id.</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var medicationsModel = new MedicationsModel { MedicationItem = new MedicationItem { Id = id } };
        var result = await _medicationBL.Delete(medicationsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
