using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.PayerTransition;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayerTransitionsController : ControllerBase
{
    private readonly IPayerTransitionBL _payerTransitionBL;
    private readonly IPayerTransitionValidationService _payerTransitionValidationService;

    public PayerTransitionsController(IPayerTransitionBL payerTransitionBL, IPayerTransitionValidationService payerTransitionValidationService)
    {
        _payerTransitionBL = payerTransitionBL;
        _payerTransitionValidationService = payerTransitionValidationService;
    }

    /// <summary>Creates a new payer transition.</summary>
    [HttpPost]
    public async Task<ActionResult<PayerTransitionResponse>> Create([FromBody] PayerTransitionRequest payerTransitionRequest)
    {
        var payerTransitionsModel = new PayerTransitionsModel { PayerTransitionRequest = payerTransitionRequest };

        payerTransitionsModel = _payerTransitionValidationService.Validate(payerTransitionsModel);
        if (payerTransitionsModel.IsNotValid)
        {
            return BadRequest(new PayerTransitionResponse
            {
                IsNotValid = true,
                Message = payerTransitionsModel.Message,
            });
        }

        payerTransitionsModel = await _payerTransitionBL.Create(payerTransitionsModel);
        return Ok(payerTransitionsModel.PayerTransitionResponse);
    }

    /// <summary>Returns all payer transitions.</summary>
    [HttpGet]
    public async Task<ActionResult<List<PayerTransitionItem>>> GetAll()
    {
        var result = await _payerTransitionBL.GetAll(new PayerTransitionsModel());
        return Ok(result.PayerTransitionItems);
    }

    /// <summary>Returns a single payer transition by Id.</summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<PayerTransitionResponse>> GetById(long id)
    {
        var payerTransitionsModel = new PayerTransitionsModel { PayerTransitionItem = new PayerTransitionItem { Id = id } };
        var result = await _payerTransitionBL.GetById(payerTransitionsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.PayerTransitionResponse);
    }

    /// <summary>Updates an existing payer transition.</summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<PayerTransitionResponse>> Update(long id, [FromBody] PayerTransitionRequest payerTransitionRequest)
    {
        var payerTransitionsModel = new PayerTransitionsModel
        {
            PayerTransitionRequest = payerTransitionRequest,
            PayerTransitionItem = new PayerTransitionItem { Id = id }
        };

        payerTransitionsModel = _payerTransitionValidationService.Validate(payerTransitionsModel);
        if (payerTransitionsModel.IsNotValid)
        {
            return BadRequest(new PayerTransitionResponse
            {
                IsNotValid = true,
                Message = payerTransitionsModel.Message,
            });
        }

        payerTransitionsModel = await _payerTransitionBL.Update(payerTransitionsModel);
        if (payerTransitionsModel.IsNotValid)
            return NotFound(payerTransitionsModel.Message);

        return Ok(payerTransitionsModel.PayerTransitionResponse);
    }

    /// <summary>Deletes a payer transition by Id.</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var payerTransitionsModel = new PayerTransitionsModel { PayerTransitionItem = new PayerTransitionItem { Id = id } };
        var result = await _payerTransitionBL.Delete(payerTransitionsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
