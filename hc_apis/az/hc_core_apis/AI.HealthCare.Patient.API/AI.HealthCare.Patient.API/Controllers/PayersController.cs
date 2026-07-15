using AI.HealthCare.Patient.API.Shared;
using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Payer;
using AI.HealthCare.Patient.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayersController : ControllerBase
{
    private readonly IPayerBL _payerBL;
    private readonly IPayerValidationService _payerValidationService;
    private readonly ICsvFileValidator _csvFileValidator;

    public PayersController(IPayerBL payerBL, IPayerValidationService payerValidationService, ICsvFileValidator csvFileValidator)
    {
        _payerBL = payerBL;
        _payerValidationService = payerValidationService;
        _csvFileValidator = csvFileValidator;
    }

    /// <summary>Creates a new payer.</summary>
    [HttpPost]
    public async Task<ActionResult<PayerResponse>> Create([FromBody] PayerRequest payerRequest)
    {
        var payersModel = new PayersModel { PayerRequest = payerRequest };

        payersModel = _payerValidationService.Validate(payersModel);
        if (payersModel.IsNotValid)
        {
            return BadRequest(new PayerResponse
            {
                IsNotValid = true,
                Message = payersModel.Message,
            });
        }

        payersModel = await _payerBL.Create(payersModel);
        return Ok(payersModel.PayerResponse);
    }

    /// <summary>Returns all payers.</summary>
    [HttpGet]
    public async Task<ActionResult<List<PayerItem>>> GetAll()
    {
        var result = await _payerBL.GetAll(new PayersModel());
        return Ok(result.PayerItems);
    }

    /// <summary>Returns a single payer by Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PayerResponse>> GetById(Guid id)
    {
        var payersModel = new PayersModel { PayerItem = new PayerItem { Id = id } };
        var result = await _payerBL.GetById(payersModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.PayerResponse);
    }

    /// <summary>Updates an existing payer.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PayerResponse>> Update(Guid id, [FromBody] PayerRequest payerRequest)
    {
        var payersModel = new PayersModel
        {
            PayerRequest = payerRequest,
            PayerItem = new PayerItem { Id = id }
        };

        payersModel = _payerValidationService.Validate(payersModel);
        if (payersModel.IsNotValid)
        {
            return BadRequest(new PayerResponse
            {
                IsNotValid = true,
                Message = payersModel.Message,
            });
        }

        payersModel = await _payerBL.Update(payersModel);
        if (payersModel.IsNotValid)
            return NotFound(payersModel.Message);

        return Ok(payersModel.PayerResponse);
    }

    /// <summary>Bulk imports payers from a CSV file (Synthea payers.csv format).</summary>
    [HttpPost("import")]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ImportResult>> Import(IFormFile file)
    {
        var (isValid, errorMessage) = _csvFileValidator.Validate(file);
        if (!isValid)
            return BadRequest(errorMessage);

        var result = await _payerBL.Import(file.OpenReadStream());
        return Ok(result);
    }

    /// <summary>Bulk upserts payers from a CSV file (Synthea payers.csv format). Rows whose Id already exists are updated in place; new Ids are inserted.</summary>
    [HttpPost("import/upsert")]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ImportResult>> ImportUpsert(IFormFile file)
    {
        var (isValid, errorMessage) = _csvFileValidator.Validate(file);
        if (!isValid)
            return BadRequest(errorMessage);

        var result = await _payerBL.ImportUpsert(file.OpenReadStream());
        return Ok(result);
    }

    /// <summary>Deletes a payer by Id.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var payersModel = new PayersModel { PayerItem = new PayerItem { Id = id } };
        var result = await _payerBL.Delete(payersModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
