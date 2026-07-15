using AI.HealthCare.Patient.API.Shared;
using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Claim;
using AI.HealthCare.Patient.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimBL _claimBL;
    private readonly IClaimValidationService _claimValidationService;
    private readonly ICsvFileValidator _csvFileValidator;

    public ClaimsController(IClaimBL claimBL, IClaimValidationService claimValidationService, ICsvFileValidator csvFileValidator)
    {
        _claimBL = claimBL;
        _claimValidationService = claimValidationService;
        _csvFileValidator = csvFileValidator;
    }

    /// <summary>Creates a new claim.</summary>
    [HttpPost]
    public async Task<ActionResult<ClaimResponse>> Create([FromBody] ClaimRequest claimRequest)
    {
        var claimsModel = new ClaimsModel { ClaimRequest = claimRequest };

        claimsModel = _claimValidationService.Validate(claimsModel);
        if (claimsModel.IsNotValid)
        {
            return BadRequest(new ClaimResponse
            {
                IsNotValid = true,
                Message = claimsModel.Message,
            });
        }

        claimsModel = await _claimBL.Create(claimsModel);
        return Ok(claimsModel.ClaimResponse);
    }

    /// <summary>Returns all claims.</summary>
    [HttpGet]
    public async Task<ActionResult<List<ClaimItem>>> GetAll()
    {
        var result = await _claimBL.GetAll(new ClaimsModel());
        return Ok(result.ClaimItems);
    }

    /// <summary>Returns all claims for a given patient.</summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<List<ClaimItem>>> GetByPatientId(Guid patientId)
    {
        var result = await _claimBL.GetByPatientId(patientId);
        return Ok(result.ClaimItems);
    }

    /// <summary>Returns a single claim by Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClaimResponse>> GetById(Guid id)
    {
        var claimsModel = new ClaimsModel { ClaimItem = new ClaimItem { Id = id } };
        var result = await _claimBL.GetById(claimsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.ClaimResponse);
    }

    /// <summary>Updates an existing claim.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClaimResponse>> Update(Guid id, [FromBody] ClaimRequest claimRequest)
    {
        var claimsModel = new ClaimsModel
        {
            ClaimRequest = claimRequest,
            ClaimItem = new ClaimItem { Id = id }
        };

        claimsModel = _claimValidationService.Validate(claimsModel);
        if (claimsModel.IsNotValid)
        {
            return BadRequest(new ClaimResponse
            {
                IsNotValid = true,
                Message = claimsModel.Message,
            });
        }

        claimsModel = await _claimBL.Update(claimsModel);
        if (claimsModel.IsNotValid)
            return NotFound(claimsModel.Message);

        return Ok(claimsModel.ClaimResponse);
    }

    /// <summary>Bulk imports claims from a CSV file (Synthea claims.csv format). Matching Patient and Provider records must already exist.</summary>
    [HttpPost("import")]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ImportResult>> Import(IFormFile file)
    {
        var (isValid, errorMessage) = _csvFileValidator.Validate(file);
        if (!isValid)
            return BadRequest(errorMessage);

        var result = await _claimBL.Import(file.OpenReadStream());
        return Ok(result);
    }

    /// <summary>Deletes a claim by Id.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var claimsModel = new ClaimsModel { ClaimItem = new ClaimItem { Id = id } };
        var result = await _claimBL.Delete(claimsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
