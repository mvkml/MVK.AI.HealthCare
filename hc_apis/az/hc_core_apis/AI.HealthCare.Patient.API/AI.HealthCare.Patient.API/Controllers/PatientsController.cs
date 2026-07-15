using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Patient;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientBL _patientBL;
    private readonly IPatientValidationService _patientValidationService;

    public PatientsController(IPatientBL patientBL, IPatientValidationService patientValidationService)
    {
        _patientBL = patientBL;
        _patientValidationService = patientValidationService;
    }

    /// <summary>Creates a new patient. Set includePii=true to receive masked Ssn/Drivers/Passport in the response (demo/learning purposes only).</summary>
    [HttpPost]
    public async Task<ActionResult<PatientResponse>> Create([FromBody] PatientRequest patientRequest, [FromQuery] bool includePii = false)
    {
        var patientsModel = new PatientsModel { PatientRequest = patientRequest, IncludePii = includePii };

        patientsModel = _patientValidationService.Validate(patientsModel);
        if (patientsModel.IsNotValid)
        {
            return BadRequest(new PatientResponse
            {
                IsNotValid = true,
                Message = patientsModel.Message,
            });
        }

        patientsModel = await _patientBL.Create(patientsModel);
        return Ok(patientsModel.PatientResponse);
    }

    /// <summary>Returns all patients.</summary>
    [HttpGet]
    public async Task<ActionResult<List<PatientItem>>> GetAll()
    {
        var result = await _patientBL.GetAll(new PatientsModel());
        return Ok(result.PatientItems);
    }

    /// <summary>Returns a single patient by Id. Set includePii=true to receive masked Ssn/Drivers/Passport in the response (demo/learning purposes only).</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PatientResponse>> GetById(Guid id, [FromQuery] bool includePii = false)
    {
        var patientsModel = new PatientsModel { PatientItem = new PatientItem { Id = id }, IncludePii = includePii };
        var result = await _patientBL.GetById(patientsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.PatientResponse);
    }

    /// <summary>Updates an existing patient. Set includePii=true to receive masked Ssn/Drivers/Passport in the response (demo/learning purposes only).</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PatientResponse>> Update(Guid id, [FromBody] PatientRequest patientRequest, [FromQuery] bool includePii = false)
    {
        var patientsModel = new PatientsModel
        {
            PatientRequest = patientRequest,
            PatientItem = new PatientItem { Id = id },
            IncludePii = includePii
        };

        patientsModel = _patientValidationService.Validate(patientsModel);
        if (patientsModel.IsNotValid)
        {
            return BadRequest(new PatientResponse
            {
                IsNotValid = true,
                Message = patientsModel.Message,
            });
        }

        patientsModel = await _patientBL.Update(patientsModel);
        if (patientsModel.IsNotValid)
            return NotFound(patientsModel.Message);

        return Ok(patientsModel.PatientResponse);
    }

    /// <summary>Deletes a patient by Id.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var patientsModel = new PatientsModel { PatientItem = new PatientItem { Id = id } };
        var result = await _patientBL.Delete(patientsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
