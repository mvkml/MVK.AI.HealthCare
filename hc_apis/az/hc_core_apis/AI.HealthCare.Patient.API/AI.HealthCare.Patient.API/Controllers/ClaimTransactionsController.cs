using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.ClaimTransaction;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClaimTransactionsController : ControllerBase
{
    private readonly IClaimTransactionBL _claimTransactionBL;
    private readonly IClaimTransactionValidationService _claimTransactionValidationService;

    public ClaimTransactionsController(IClaimTransactionBL claimTransactionBL, IClaimTransactionValidationService claimTransactionValidationService)
    {
        _claimTransactionBL = claimTransactionBL;
        _claimTransactionValidationService = claimTransactionValidationService;
    }

    /// <summary>Creates a new claim transaction.</summary>
    [HttpPost]
    public async Task<ActionResult<ClaimTransactionResponse>> Create([FromBody] ClaimTransactionRequest claimTransactionRequest)
    {
        var claimTransactionsModel = new ClaimTransactionsModel { ClaimTransactionRequest = claimTransactionRequest };

        claimTransactionsModel = _claimTransactionValidationService.Validate(claimTransactionsModel);
        if (claimTransactionsModel.IsNotValid)
        {
            return BadRequest(new ClaimTransactionResponse
            {
                IsNotValid = true,
                Message = claimTransactionsModel.Message,
            });
        }

        claimTransactionsModel = await _claimTransactionBL.Create(claimTransactionsModel);
        return Ok(claimTransactionsModel.ClaimTransactionResponse);
    }

    /// <summary>Returns all claim transactions.</summary>
    [HttpGet]
    public async Task<ActionResult<List<ClaimTransactionItem>>> GetAll()
    {
        var result = await _claimTransactionBL.GetAll(new ClaimTransactionsModel());
        return Ok(result.ClaimTransactionItems);
    }

    /// <summary>Returns all claim transactions for a given claim.</summary>
    [HttpGet("by-claim/{claimId:guid}")]
    public async Task<ActionResult<List<ClaimTransactionItem>>> GetByClaimId(Guid claimId)
    {
        var result = await _claimTransactionBL.GetByClaimId(claimId);
        return Ok(result.ClaimTransactionItems);
    }

    /// <summary>Returns a single claim transaction by Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClaimTransactionResponse>> GetById(Guid id)
    {
        var claimTransactionsModel = new ClaimTransactionsModel { ClaimTransactionItem = new ClaimTransactionItem { Id = id } };
        var result = await _claimTransactionBL.GetById(claimTransactionsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.ClaimTransactionResponse);
    }

    /// <summary>Updates an existing claim transaction.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClaimTransactionResponse>> Update(Guid id, [FromBody] ClaimTransactionRequest claimTransactionRequest)
    {
        var claimTransactionsModel = new ClaimTransactionsModel
        {
            ClaimTransactionRequest = claimTransactionRequest,
            ClaimTransactionItem = new ClaimTransactionItem { Id = id }
        };

        claimTransactionsModel = _claimTransactionValidationService.Validate(claimTransactionsModel);
        if (claimTransactionsModel.IsNotValid)
        {
            return BadRequest(new ClaimTransactionResponse
            {
                IsNotValid = true,
                Message = claimTransactionsModel.Message,
            });
        }

        claimTransactionsModel = await _claimTransactionBL.Update(claimTransactionsModel);
        if (claimTransactionsModel.IsNotValid)
            return NotFound(claimTransactionsModel.Message);

        return Ok(claimTransactionsModel.ClaimTransactionResponse);
    }

    /// <summary>Deletes a claim transaction by Id.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var claimTransactionsModel = new ClaimTransactionsModel { ClaimTransactionItem = new ClaimTransactionItem { Id = id } };
        var result = await _claimTransactionBL.Delete(claimTransactionsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
