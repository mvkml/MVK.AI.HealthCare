using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Allergy;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AllergiesController : ControllerBase
{
    private readonly IAllergyBL _allergyBL;
    private readonly IAllergyValidationService _allergyValidationService;

    public AllergiesController(IAllergyBL allergyBL, IAllergyValidationService allergyValidationService)
    {
        _allergyBL = allergyBL;
        _allergyValidationService = allergyValidationService;
    }

    /// <summary>Creates a new allergy.</summary>
    [HttpPost]
    public async Task<ActionResult<AllergyResponse>> Create([FromBody] AllergyRequest allergyRequest)
    {
        var allergiesModel = new AllergiesModel { AllergyRequest = allergyRequest };

        allergiesModel = _allergyValidationService.Validate(allergiesModel);
        if (allergiesModel.IsNotValid)
        {
            return BadRequest(new AllergyResponse
            {
                IsNotValid = true,
                Message = allergiesModel.Message,
            });
        }

        allergiesModel = await _allergyBL.Create(allergiesModel);
        return Ok(allergiesModel.AllergyResponse);
    }

    /// <summary>Returns all allergies.</summary>
    [HttpGet]
    public async Task<ActionResult<List<AllergyItem>>> GetAll()
    {
        var result = await _allergyBL.GetAll(new AllergiesModel());
        return Ok(result.AllergyItems);
    }

    /// <summary>Returns all allergies for a given patient.</summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<List<AllergyItem>>> GetByPatientId(Guid patientId)
    {
        var result = await _allergyBL.GetByPatientId(patientId);
        return Ok(result.AllergyItems);
    }

    /// <summary>Returns a single allergy by Id.</summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<AllergyResponse>> GetById(long id)
    {
        var allergiesModel = new AllergiesModel { AllergyItem = new AllergyItem { Id = id } };
        var result = await _allergyBL.GetById(allergiesModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.AllergyResponse);
    }

    /// <summary>Updates an existing allergy.</summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<AllergyResponse>> Update(long id, [FromBody] AllergyRequest allergyRequest)
    {
        var allergiesModel = new AllergiesModel
        {
            AllergyRequest = allergyRequest,
            AllergyItem = new AllergyItem { Id = id }
        };

        allergiesModel = _allergyValidationService.Validate(allergiesModel);
        if (allergiesModel.IsNotValid)
        {
            return BadRequest(new AllergyResponse
            {
                IsNotValid = true,
                Message = allergiesModel.Message,
            });
        }

        allergiesModel = await _allergyBL.Update(allergiesModel);
        if (allergiesModel.IsNotValid)
            return NotFound(allergiesModel.Message);

        return Ok(allergiesModel.AllergyResponse);
    }

    /// <summary>Deletes an allergy by Id.</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var allergiesModel = new AllergiesModel { AllergyItem = new AllergyItem { Id = id } };
        var result = await _allergyBL.Delete(allergiesModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
