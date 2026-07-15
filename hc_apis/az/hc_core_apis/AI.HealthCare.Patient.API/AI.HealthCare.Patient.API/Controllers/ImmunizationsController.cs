using AI.HealthCare.Patient.API.Shared;
using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Immunization;
using AI.HealthCare.Patient.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImmunizationsController : ControllerBase
{
    private readonly IImmunizationBL _immunizationBL;
    private readonly IImmunizationValidationService _immunizationValidationService;
    private readonly ICsvFileValidator _csvFileValidator;

    public ImmunizationsController(IImmunizationBL immunizationBL, IImmunizationValidationService immunizationValidationService, ICsvFileValidator csvFileValidator)
    {
        _immunizationBL = immunizationBL;
        _immunizationValidationService = immunizationValidationService;
        _csvFileValidator = csvFileValidator;
    }

    /// <summary>Creates a new immunization.</summary>
    [HttpPost]
    public async Task<ActionResult<ImmunizationResponse>> Create([FromBody] ImmunizationRequest immunizationRequest)
    {
        var immunizationsModel = new ImmunizationsModel { ImmunizationRequest = immunizationRequest };

        immunizationsModel = _immunizationValidationService.Validate(immunizationsModel);
        if (immunizationsModel.IsNotValid)
        {
            return BadRequest(new ImmunizationResponse
            {
                IsNotValid = true,
                Message = immunizationsModel.Message,
            });
        }

        immunizationsModel = await _immunizationBL.Create(immunizationsModel);
        return Ok(immunizationsModel.ImmunizationResponse);
    }

    /// <summary>Returns all immunizations.</summary>
    [HttpGet]
    public async Task<ActionResult<List<ImmunizationItem>>> GetAll()
    {
        var result = await _immunizationBL.GetAll(new ImmunizationsModel());
        return Ok(result.ImmunizationItems);
    }

    /// <summary>Returns all immunizations for a given patient.</summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<List<ImmunizationItem>>> GetByPatientId(Guid patientId)
    {
        var result = await _immunizationBL.GetByPatientId(patientId);
        return Ok(result.ImmunizationItems);
    }

    /// <summary>Returns a single immunization by Id.</summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<ImmunizationResponse>> GetById(long id)
    {
        var immunizationsModel = new ImmunizationsModel { ImmunizationItem = new ImmunizationItem { Id = id } };
        var result = await _immunizationBL.GetById(immunizationsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.ImmunizationResponse);
    }

    /// <summary>Updates an existing immunization.</summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ImmunizationResponse>> Update(long id, [FromBody] ImmunizationRequest immunizationRequest)
    {
        var immunizationsModel = new ImmunizationsModel
        {
            ImmunizationRequest = immunizationRequest,
            ImmunizationItem = new ImmunizationItem { Id = id }
        };

        immunizationsModel = _immunizationValidationService.Validate(immunizationsModel);
        if (immunizationsModel.IsNotValid)
        {
            return BadRequest(new ImmunizationResponse
            {
                IsNotValid = true,
                Message = immunizationsModel.Message,
            });
        }

        immunizationsModel = await _immunizationBL.Update(immunizationsModel);
        if (immunizationsModel.IsNotValid)
            return NotFound(immunizationsModel.Message);

        return Ok(immunizationsModel.ImmunizationResponse);
    }

    /// <summary>Bulk imports immunizations from a CSV file (Synthea immunizations.csv format). Matching Patient and Encounter records must already exist.</summary>
    [HttpPost("import")]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ImportResult>> Import(IFormFile file)
    {
        var (isValid, errorMessage) = _csvFileValidator.Validate(file);
        if (!isValid)
            return BadRequest(errorMessage);

        var result = await _immunizationBL.Import(file.OpenReadStream());
        return Ok(result);
    }

    /// <summary>Bulk upserts immunizations from a CSV file (Synthea immunizations.csv format). Rows matching an existing Patient+Encounter+Code combination are updated in place; others are inserted. Matching Patient and Encounter records must already exist.</summary>
    [HttpPost("import/upsert")]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ImportResult>> ImportUpsert(IFormFile file)
    {
        var (isValid, errorMessage) = _csvFileValidator.Validate(file);
        if (!isValid)
            return BadRequest(errorMessage);

        var result = await _immunizationBL.ImportUpsert(file.OpenReadStream());
        return Ok(result);
    }

    /// <summary>Deletes an immunization by Id.</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var immunizationsModel = new ImmunizationsModel { ImmunizationItem = new ImmunizationItem { Id = id } };
        var result = await _immunizationBL.Delete(immunizationsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
