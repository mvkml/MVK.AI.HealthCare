using AI.HealthCare.Patient.API.Shared;
using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Procedure;
using AI.HealthCare.Patient.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProceduresController : ControllerBase
{
    private readonly IProcedureBL _procedureBL;
    private readonly IProcedureValidationService _procedureValidationService;
    private readonly ICsvFileValidator _csvFileValidator;

    public ProceduresController(IProcedureBL procedureBL, IProcedureValidationService procedureValidationService, ICsvFileValidator csvFileValidator)
    {
        _procedureBL = procedureBL;
        _procedureValidationService = procedureValidationService;
        _csvFileValidator = csvFileValidator;
    }

    /// <summary>Creates a new procedure.</summary>
    [HttpPost]
    public async Task<ActionResult<ProcedureResponse>> Create([FromBody] ProcedureRequest procedureRequest)
    {
        var proceduresModel = new ProceduresModel { ProcedureRequest = procedureRequest };

        proceduresModel = _procedureValidationService.Validate(proceduresModel);
        if (proceduresModel.IsNotValid)
        {
            return BadRequest(new ProcedureResponse
            {
                IsNotValid = true,
                Message = proceduresModel.Message,
            });
        }

        proceduresModel = await _procedureBL.Create(proceduresModel);
        return Ok(proceduresModel.ProcedureResponse);
    }

    /// <summary>Returns all procedures.</summary>
    [HttpGet]
    public async Task<ActionResult<List<ProcedureItem>>> GetAll()
    {
        var result = await _procedureBL.GetAll(new ProceduresModel());
        return Ok(result.ProcedureItems);
    }

    /// <summary>Returns all procedures for a given patient.</summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<List<ProcedureItem>>> GetByPatientId(Guid patientId)
    {
        var result = await _procedureBL.GetByPatientId(patientId);
        return Ok(result.ProcedureItems);
    }

    /// <summary>Returns a single procedure by Id.</summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<ProcedureResponse>> GetById(long id)
    {
        var proceduresModel = new ProceduresModel { ProcedureItem = new ProcedureItem { Id = id } };
        var result = await _procedureBL.GetById(proceduresModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.ProcedureResponse);
    }

    /// <summary>Updates an existing procedure.</summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ProcedureResponse>> Update(long id, [FromBody] ProcedureRequest procedureRequest)
    {
        var proceduresModel = new ProceduresModel
        {
            ProcedureRequest = procedureRequest,
            ProcedureItem = new ProcedureItem { Id = id }
        };

        proceduresModel = _procedureValidationService.Validate(proceduresModel);
        if (proceduresModel.IsNotValid)
        {
            return BadRequest(new ProcedureResponse
            {
                IsNotValid = true,
                Message = proceduresModel.Message,
            });
        }

        proceduresModel = await _procedureBL.Update(proceduresModel);
        if (proceduresModel.IsNotValid)
            return NotFound(proceduresModel.Message);

        return Ok(proceduresModel.ProcedureResponse);
    }

    /// <summary>Bulk imports procedures from a CSV file (Synthea procedures.csv format). Matching Patient and Encounter records must already exist.</summary>
    [HttpPost("import")]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ImportResult>> Import(IFormFile file)
    {
        var (isValid, errorMessage) = _csvFileValidator.Validate(file);
        if (!isValid)
            return BadRequest(errorMessage);

        var result = await _procedureBL.Import(file.OpenReadStream());
        return Ok(result);
    }

    /// <summary>Bulk upserts procedures from a CSV file (Synthea procedures.csv format). Rows matching an existing Patient+Encounter+Code combination are updated in place; others are inserted. Matching Patient and Encounter records must already exist.</summary>
    [HttpPost("import/upsert")]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ImportResult>> ImportUpsert(IFormFile file)
    {
        var (isValid, errorMessage) = _csvFileValidator.Validate(file);
        if (!isValid)
            return BadRequest(errorMessage);

        var result = await _procedureBL.ImportUpsert(file.OpenReadStream());
        return Ok(result);
    }

    /// <summary>Deletes a procedure by Id.</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var proceduresModel = new ProceduresModel { ProcedureItem = new ProcedureItem { Id = id } };
        var result = await _procedureBL.Delete(proceduresModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
