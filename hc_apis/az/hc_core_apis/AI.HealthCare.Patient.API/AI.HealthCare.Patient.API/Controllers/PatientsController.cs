using AI.HealthCare.Patient.API.Shared;
using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Patient;
using AI.HealthCare.Patient.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientBL _patientBL;
    private readonly IPatientValidationService _patientValidationService;
    private readonly ICsvFileValidator _csvFileValidator;

    public PatientsController(IPatientBL patientBL, IPatientValidationService patientValidationService, ICsvFileValidator csvFileValidator)
    {
        _patientBL = patientBL;
        _patientValidationService = patientValidationService;
        _csvFileValidator = csvFileValidator;
    }

    /// <summary>Creates a new patient. Set includePii=true to receive masked Ssn/Drivers/Passport in the response (demo/learning purposes only).</summary>
    [HttpPost]
    public async Task<ActionResult<PatientResponse>> Create([FromBody] PatientRequest patientRequest, [FromQuery] bool includePii = false)
    {
        var patientsModel = new PatientsModel { PatientRequest = patientRequest, IncludePii = includePii };

        patientsModel = _patientValidationService.Validate(patientsModel);
        if (patientsModel.IsNotValid)
        {
            return BadRequest(new PatientResponse
            {
                IsNotValid = true,
                Message = patientsModel.Message,
            });
        }

        patientsModel = await _patientBL.Create(patientsModel);
        return Ok(patientsModel.PatientResponse);
    }

    /// <summary>Returns all patients.</summary>
    [HttpGet]
    public async Task<ActionResult<List<PatientItem>>> GetAll()
    {
        var result = await _patientBL.GetAll(new PatientsModel());
        return Ok(result.PatientItems);
    }

    /// <summary>Returns a single patient by Id. Set includePii=true to receive masked Ssn/Drivers/Passport in the response (demo/learning purposes only).</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PatientResponse>> GetById(Guid id, [FromQuery] bool includePii = false)
    {
        var patientsModel = new PatientsModel { PatientItem = new PatientItem { Id = id }, IncludePii = includePii };
        var result = await _patientBL.GetById(patientsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.PatientResponse);
    }

    /// <summary>Updates an existing patient. Set includePii=true to receive masked Ssn/Drivers/Passport in the response (demo/learning purposes only).</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PatientResponse>> Update(Guid id, [FromBody] PatientRequest patientRequest, [FromQuery] bool includePii = false)
    {
        var patientsModel = new PatientsModel
        {
            PatientRequest = patientRequest,
            PatientItem = new PatientItem { Id = id },
            IncludePii = includePii
        };

        patientsModel = _patientValidationService.Validate(patientsModel);
        if (patientsModel.IsNotValid)
        {
            return BadRequest(new PatientResponse
            {
                IsNotValid = true,
                Message = patientsModel.Message,
            });
        }

        patientsModel = await _patientBL.Update(patientsModel);
        if (patientsModel.IsNotValid)
            return NotFound(patientsModel.Message);

        return Ok(patientsModel.PatientResponse);
    }

    /// <summary>Bulk imports patients from a CSV file (Synthea patients.csv format). The Id column is preserved as-is so other entities' FK references resolve correctly.</summary>
    [HttpPost("import")]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ImportResult>> Import(IFormFile file)
    {
        var (isValid, errorMessage) = _csvFileValidator.Validate(file);
        if (!isValid)
            return BadRequest(errorMessage);

        var result = await _patientBL.Import(file.OpenReadStream());
        return Ok(result);
    }

    /// <summary>Bulk upserts patients from a CSV file (Synthea patients.csv format). Rows whose Id already exists are updated in place; new Ids are inserted.</summary>
    [HttpPost("import/upsert")]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ImportResult>> ImportUpsert(IFormFile file)
    {
        var (isValid, errorMessage) = _csvFileValidator.Validate(file);
        if (!isValid)
            return BadRequest(errorMessage);

        var result = await _patientBL.ImportUpsert(file.OpenReadStream());
        return Ok(result);
    }

    /// <summary>Deletes a patient by Id.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var patientsModel = new PatientsModel { PatientItem = new PatientItem { Id = id } };
        var result = await _patientBL.Delete(patientsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
