using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Supply;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliesController : ControllerBase
{
    private readonly ISupplyBL _supplyBL;
    private readonly ISupplyValidationService _supplyValidationService;

    public SuppliesController(ISupplyBL supplyBL, ISupplyValidationService supplyValidationService)
    {
        _supplyBL = supplyBL;
        _supplyValidationService = supplyValidationService;
    }

    /// <summary>Creates a new supply.</summary>
    [HttpPost]
    public async Task<ActionResult<SupplyResponse>> Create([FromBody] SupplyRequest supplyRequest)
    {
        var suppliesModel = new SuppliesModel { SupplyRequest = supplyRequest };

        suppliesModel = _supplyValidationService.Validate(suppliesModel);
        if (suppliesModel.IsNotValid)
        {
            return BadRequest(new SupplyResponse
            {
                IsNotValid = true,
                Message = suppliesModel.Message,
            });
        }

        suppliesModel = await _supplyBL.Create(suppliesModel);
        return Ok(suppliesModel.SupplyResponse);
    }

    /// <summary>Returns all supplies.</summary>
    [HttpGet]
    public async Task<ActionResult<List<SupplyItem>>> GetAll()
    {
        var result = await _supplyBL.GetAll(new SuppliesModel());
        return Ok(result.SupplyItems);
    }

    /// <summary>Returns all supplies for a given patient.</summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<List<SupplyItem>>> GetByPatientId(Guid patientId)
    {
        var result = await _supplyBL.GetByPatientId(patientId);
        return Ok(result.SupplyItems);
    }

    /// <summary>Returns a single supply by Id.</summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<SupplyResponse>> GetById(long id)
    {
        var suppliesModel = new SuppliesModel { SupplyItem = new SupplyItem { Id = id } };
        var result = await _supplyBL.GetById(suppliesModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.SupplyResponse);
    }

    /// <summary>Updates an existing supply.</summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<SupplyResponse>> Update(long id, [FromBody] SupplyRequest supplyRequest)
    {
        var suppliesModel = new SuppliesModel
        {
            SupplyRequest = supplyRequest,
            SupplyItem = new SupplyItem { Id = id }
        };

        suppliesModel = _supplyValidationService.Validate(suppliesModel);
        if (suppliesModel.IsNotValid)
        {
            return BadRequest(new SupplyResponse
            {
                IsNotValid = true,
                Message = suppliesModel.Message,
            });
        }

        suppliesModel = await _supplyBL.Update(suppliesModel);
        if (suppliesModel.IsNotValid)
            return NotFound(suppliesModel.Message);

        return Ok(suppliesModel.SupplyResponse);
    }

    /// <summary>Deletes a supply by Id.</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var suppliesModel = new SuppliesModel { SupplyItem = new SupplyItem { Id = id } };
        var result = await _supplyBL.Delete(suppliesModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
