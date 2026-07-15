using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Observation;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ObservationsController : ControllerBase
{
    private readonly IObservationBL _observationBL;
    private readonly IObservationValidationService _observationValidationService;

    public ObservationsController(IObservationBL observationBL, IObservationValidationService observationValidationService)
    {
        _observationBL = observationBL;
        _observationValidationService = observationValidationService;
    }

    /// <summary>Creates a new observation.</summary>
    [HttpPost]
    public async Task<ActionResult<ObservationResponse>> Create([FromBody] ObservationRequest observationRequest)
    {
        var observationsModel = new ObservationsModel { ObservationRequest = observationRequest };

        observationsModel = _observationValidationService.Validate(observationsModel);
        if (observationsModel.IsNotValid)
        {
            return BadRequest(new ObservationResponse
            {
                IsNotValid = true,
                Message = observationsModel.Message,
            });
        }

        observationsModel = await _observationBL.Create(observationsModel);
        return Ok(observationsModel.ObservationResponse);
    }

    /// <summary>Returns all observations.</summary>
    [HttpGet]
    public async Task<ActionResult<List<ObservationItem>>> GetAll()
    {
        var result = await _observationBL.GetAll(new ObservationsModel());
        return Ok(result.ObservationItems);
    }

    /// <summary>Returns all observations for a given patient.</summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<List<ObservationItem>>> GetByPatientId(Guid patientId)
    {
        var result = await _observationBL.GetByPatientId(patientId);
        return Ok(result.ObservationItems);
    }

    /// <summary>Returns a single observation by Id.</summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<ObservationResponse>> GetById(long id)
    {
        var observationsModel = new ObservationsModel { ObservationItem = new ObservationItem { Id = id } };
        var result = await _observationBL.GetById(observationsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.ObservationResponse);
    }

    /// <summary>Updates an existing observation.</summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ObservationResponse>> Update(long id, [FromBody] ObservationRequest observationRequest)
    {
        var observationsModel = new ObservationsModel
        {
            ObservationRequest = observationRequest,
            ObservationItem = new ObservationItem { Id = id }
        };

        observationsModel = _observationValidationService.Validate(observationsModel);
        if (observationsModel.IsNotValid)
        {
            return BadRequest(new ObservationResponse
            {
                IsNotValid = true,
                Message = observationsModel.Message,
            });
        }

        observationsModel = await _observationBL.Update(observationsModel);
        if (observationsModel.IsNotValid)
            return NotFound(observationsModel.Message);

        return Ok(observationsModel.ObservationResponse);
    }

    /// <summary>Deletes an observation by Id.</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var observationsModel = new ObservationsModel { ObservationItem = new ObservationItem { Id = id } };
        var result = await _observationBL.Delete(observationsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
