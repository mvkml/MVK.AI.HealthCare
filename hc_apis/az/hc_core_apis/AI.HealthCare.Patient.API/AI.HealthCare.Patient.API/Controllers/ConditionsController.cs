using AI.HealthCare.Patient.API.Shared;
using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Condition;
using AI.HealthCare.Patient.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConditionsController : ControllerBase
{
    private readonly IConditionBL _conditionBL;
    private readonly IConditionValidationService _conditionValidationService;
    private readonly ICsvFileValidator _csvFileValidator;

    public ConditionsController(IConditionBL conditionBL, IConditionValidationService conditionValidationService, ICsvFileValidator csvFileValidator)
    {
        _conditionBL = conditionBL;
        _conditionValidationService = conditionValidationService;
        _csvFileValidator = csvFileValidator;
    }

    /// <summary>Creates a new condition.</summary>
    [HttpPost]
    public async Task<ActionResult<ConditionResponse>> Create([FromBody] ConditionRequest conditionRequest)
    {
        var conditionsModel = new ConditionsModel { ConditionRequest = conditionRequest };

        conditionsModel = _conditionValidationService.Validate(conditionsModel);
        if (conditionsModel.IsNotValid)
        {
            return BadRequest(new ConditionResponse
            {
                IsNotValid = true,
                Message = conditionsModel.Message,
            });
        }

        conditionsModel = await _conditionBL.Create(conditionsModel);
        return Ok(conditionsModel.ConditionResponse);
    }

    /// <summary>Returns all conditions.</summary>
    [HttpGet]
    public async Task<ActionResult<List<ConditionItem>>> GetAll()
    {
        var result = await _conditionBL.GetAll(new ConditionsModel());
        return Ok(result.ConditionItems);
    }

    /// <summary>Returns all conditions for a given patient.</summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<List<ConditionItem>>> GetByPatientId(Guid patientId)
    {
        var result = await _conditionBL.GetByPatientId(patientId);
        return Ok(result.ConditionItems);
    }

    /// <summary>Returns a single condition by Id.</summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<ConditionResponse>> GetById(long id)
    {
        var conditionsModel = new ConditionsModel { ConditionItem = new ConditionItem { Id = id } };
        var result = await _conditionBL.GetById(conditionsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.ConditionResponse);
    }

    /// <summary>Updates an existing condition.</summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ConditionResponse>> Update(long id, [FromBody] ConditionRequest conditionRequest)
    {
        var conditionsModel = new ConditionsModel
        {
            ConditionRequest = conditionRequest,
            ConditionItem = new ConditionItem { Id = id }
        };

        conditionsModel = _conditionValidationService.Validate(conditionsModel);
        if (conditionsModel.IsNotValid)
        {
            return BadRequest(new ConditionResponse
            {
                IsNotValid = true,
                Message = conditionsModel.Message,
            });
        }

        conditionsModel = await _conditionBL.Update(conditionsModel);
        if (conditionsModel.IsNotValid)
            return NotFound(conditionsModel.Message);

        return Ok(conditionsModel.ConditionResponse);
    }

    /// <summary>Bulk imports conditions from a CSV file (Synthea conditions.csv format). Matching Patient and Encounter records must already exist.</summary>
    [HttpPost("import")]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ImportResult>> Import(IFormFile file)
    {
        var (isValid, errorMessage) = _csvFileValidator.Validate(file);
        if (!isValid)
            return BadRequest(errorMessage);

        var result = await _conditionBL.Import(file.OpenReadStream());
        return Ok(result);
    }

    /// <summary>Deletes a condition by Id.</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var conditionsModel = new ConditionsModel { ConditionItem = new ConditionItem { Id = id } };
        var result = await _conditionBL.Delete(conditionsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
