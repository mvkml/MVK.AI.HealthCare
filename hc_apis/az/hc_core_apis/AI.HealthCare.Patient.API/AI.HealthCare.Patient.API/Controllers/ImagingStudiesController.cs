using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.ImagingStudy;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagingStudiesController : ControllerBase
{
    private readonly IImagingStudyBL _imagingStudyBL;
    private readonly IImagingStudyValidationService _imagingStudyValidationService;

    public ImagingStudiesController(IImagingStudyBL imagingStudyBL, IImagingStudyValidationService imagingStudyValidationService)
    {
        _imagingStudyBL = imagingStudyBL;
        _imagingStudyValidationService = imagingStudyValidationService;
    }

    /// <summary>Creates a new imaging study.</summary>
    [HttpPost]
    public async Task<ActionResult<ImagingStudyResponse>> Create([FromBody] ImagingStudyRequest imagingStudyRequest)
    {
        var imagingStudiesModel = new ImagingStudiesModel { ImagingStudyRequest = imagingStudyRequest };

        imagingStudiesModel = _imagingStudyValidationService.Validate(imagingStudiesModel);
        if (imagingStudiesModel.IsNotValid)
        {
            return BadRequest(new ImagingStudyResponse
            {
                IsNotValid = true,
                Message = imagingStudiesModel.Message,
            });
        }

        imagingStudiesModel = await _imagingStudyBL.Create(imagingStudiesModel);
        return Ok(imagingStudiesModel.ImagingStudyResponse);
    }

    /// <summary>Returns all imaging studies.</summary>
    [HttpGet]
    public async Task<ActionResult<List<ImagingStudyItem>>> GetAll()
    {
        var result = await _imagingStudyBL.GetAll(new ImagingStudiesModel());
        return Ok(result.ImagingStudyItems);
    }

    /// <summary>Returns all imaging studies for a given patient.</summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<List<ImagingStudyItem>>> GetByPatientId(Guid patientId)
    {
        var result = await _imagingStudyBL.GetByPatientId(patientId);
        return Ok(result.ImagingStudyItems);
    }

    /// <summary>Returns a single imaging study by Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ImagingStudyResponse>> GetById(Guid id)
    {
        var imagingStudiesModel = new ImagingStudiesModel { ImagingStudyItem = new ImagingStudyItem { Id = id } };
        var result = await _imagingStudyBL.GetById(imagingStudiesModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.ImagingStudyResponse);
    }

    /// <summary>Updates an existing imaging study.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ImagingStudyResponse>> Update(Guid id, [FromBody] ImagingStudyRequest imagingStudyRequest)
    {
        var imagingStudiesModel = new ImagingStudiesModel
        {
            ImagingStudyRequest = imagingStudyRequest,
            ImagingStudyItem = new ImagingStudyItem { Id = id }
        };

        imagingStudiesModel = _imagingStudyValidationService.Validate(imagingStudiesModel);
        if (imagingStudiesModel.IsNotValid)
        {
            return BadRequest(new ImagingStudyResponse
            {
                IsNotValid = true,
                Message = imagingStudiesModel.Message,
            });
        }

        imagingStudiesModel = await _imagingStudyBL.Update(imagingStudiesModel);
        if (imagingStudiesModel.IsNotValid)
            return NotFound(imagingStudiesModel.Message);

        return Ok(imagingStudiesModel.ImagingStudyResponse);
    }

    /// <summary>Deletes an imaging study by Id.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var imagingStudiesModel = new ImagingStudiesModel { ImagingStudyItem = new ImagingStudyItem { Id = id } };
        var result = await _imagingStudyBL.Delete(imagingStudiesModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
