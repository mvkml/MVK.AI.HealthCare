using AI.HealthCare.Patient.API.Shared;
using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Encounter;
using AI.HealthCare.Patient.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EncountersController : ControllerBase
{
    private readonly IEncounterBL _encounterBL;
    private readonly IEncounterValidationService _encounterValidationService;
    private readonly ICsvFileValidator _csvFileValidator;

    public EncountersController(IEncounterBL encounterBL, IEncounterValidationService encounterValidationService, ICsvFileValidator csvFileValidator)
    {
        _encounterBL = encounterBL;
        _encounterValidationService = encounterValidationService;
        _csvFileValidator = csvFileValidator;
    }

    /// <summary>Creates a new encounter.</summary>
    [HttpPost]
    public async Task<ActionResult<EncounterResponse>> Create([FromBody] EncounterRequest encounterRequest)
    {
        var encountersModel = new EncountersModel { EncounterRequest = encounterRequest };

        encountersModel = _encounterValidationService.Validate(encountersModel);
        if (encountersModel.IsNotValid)
        {
            return BadRequest(new EncounterResponse
            {
                IsNotValid = true,
                Message = encountersModel.Message,
            });
        }

        encountersModel = await _encounterBL.Create(encountersModel);
        return Ok(encountersModel.EncounterResponse);
    }

    /// <summary>Returns all encounters.</summary>
    [HttpGet]
    public async Task<ActionResult<List<EncounterItem>>> GetAll()
    {
        var result = await _encounterBL.GetAll(new EncountersModel());
        return Ok(result.EncounterItems);
    }

    /// <summary>Returns all encounters for a given patient.</summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<List<EncounterItem>>> GetByPatientId(Guid patientId)
    {
        var result = await _encounterBL.GetByPatientId(patientId);
        return Ok(result.EncounterItems);
    }

    /// <summary>Returns a single encounter by Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EncounterResponse>> GetById(Guid id)
    {
        var encountersModel = new EncountersModel { EncounterItem = new EncounterItem { Id = id } };
        var result = await _encounterBL.GetById(encountersModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.EncounterResponse);
    }

    /// <summary>Updates an existing encounter.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EncounterResponse>> Update(Guid id, [FromBody] EncounterRequest encounterRequest)
    {
        var encountersModel = new EncountersModel
        {
            EncounterRequest = encounterRequest,
            EncounterItem = new EncounterItem { Id = id }
        };

        encountersModel = _encounterValidationService.Validate(encountersModel);
        if (encountersModel.IsNotValid)
        {
            return BadRequest(new EncounterResponse
            {
                IsNotValid = true,
                Message = encountersModel.Message,
            });
        }

        encountersModel = await _encounterBL.Update(encountersModel);
        if (encountersModel.IsNotValid)
            return NotFound(encountersModel.Message);

        return Ok(encountersModel.EncounterResponse);
    }

    /// <summary>Bulk imports encounters from a CSV file (Synthea encounters.csv format). Matching Patient, Organization, Provider, and Payer records must already exist.</summary>
    [HttpPost("import")]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ImportResult>> Import(IFormFile file)
    {
        var (isValid, errorMessage) = _csvFileValidator.Validate(file);
        if (!isValid)
            return BadRequest(errorMessage);

        var result = await _encounterBL.Import(file.OpenReadStream());
        return Ok(result);
    }

    /// <summary>Deletes an encounter by Id.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var encountersModel = new EncountersModel { EncounterItem = new EncounterItem { Id = id } };
        var result = await _encounterBL.Delete(encountersModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
