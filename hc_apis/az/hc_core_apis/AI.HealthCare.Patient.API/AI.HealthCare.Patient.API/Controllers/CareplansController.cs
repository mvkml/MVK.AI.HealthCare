using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Careplan;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CareplansController : ControllerBase
{
    private readonly ICareplanBL _careplanBL;
    private readonly ICareplanValidationService _careplanValidationService;

    public CareplansController(ICareplanBL careplanBL, ICareplanValidationService careplanValidationService)
    {
        _careplanBL = careplanBL;
        _careplanValidationService = careplanValidationService;
    }

    /// <summary>Creates a new careplan.</summary>
    [HttpPost]
    public async Task<ActionResult<CareplanResponse>> Create([FromBody] CareplanRequest careplanRequest)
    {
        var careplansModel = new CareplansModel { CareplanRequest = careplanRequest };

        careplansModel = _careplanValidationService.Validate(careplansModel);
        if (careplansModel.IsNotValid)
        {
            return BadRequest(new CareplanResponse
            {
                IsNotValid = true,
                Message = careplansModel.Message,
            });
        }

        careplansModel = await _careplanBL.Create(careplansModel);
        return Ok(careplansModel.CareplanResponse);
    }

    /// <summary>Returns all careplans.</summary>
    [HttpGet]
    public async Task<ActionResult<List<CareplanItem>>> GetAll()
    {
        var result = await _careplanBL.GetAll(new CareplansModel());
        return Ok(result.CareplanItems);
    }

    /// <summary>Returns all careplans for a given patient.</summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<List<CareplanItem>>> GetByPatientId(Guid patientId)
    {
        var result = await _careplanBL.GetByPatientId(patientId);
        return Ok(result.CareplanItems);
    }

    /// <summary>Returns a single careplan by Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CareplanResponse>> GetById(Guid id)
    {
        var careplansModel = new CareplansModel { CareplanItem = new CareplanItem { Id = id } };
        var result = await _careplanBL.GetById(careplansModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.CareplanResponse);
    }

    /// <summary>Updates an existing careplan.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CareplanResponse>> Update(Guid id, [FromBody] CareplanRequest careplanRequest)
    {
        var careplansModel = new CareplansModel
        {
            CareplanRequest = careplanRequest,
            CareplanItem = new CareplanItem { Id = id }
        };

        careplansModel = _careplanValidationService.Validate(careplansModel);
        if (careplansModel.IsNotValid)
        {
            return BadRequest(new CareplanResponse
            {
                IsNotValid = true,
                Message = careplansModel.Message,
            });
        }

        careplansModel = await _careplanBL.Update(careplansModel);
        if (careplansModel.IsNotValid)
            return NotFound(careplansModel.Message);

        return Ok(careplansModel.CareplanResponse);
    }

    /// <summary>Deletes a careplan by Id.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var careplansModel = new CareplansModel { CareplanItem = new CareplanItem { Id = id } };
        var result = await _careplanBL.Delete(careplansModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
