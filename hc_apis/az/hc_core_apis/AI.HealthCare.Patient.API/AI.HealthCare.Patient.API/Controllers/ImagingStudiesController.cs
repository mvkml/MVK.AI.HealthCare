using AI.HealthCare.Patient.API.Shared;
using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.ImagingStudy;
using AI.HealthCare.Patient.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagingStudiesController : ControllerBase
{
    private readonly IImagingStudyBL _imagingStudyBL;
    private readonly IImagingStudyValidationService _imagingStudyValidationService;
    private readonly ICsvFileValidator _csvFileValidator;

    public ImagingStudiesController(IImagingStudyBL imagingStudyBL, IImagingStudyValidationService imagingStudyValidationService, ICsvFileValidator csvFileValidator)
    {
        _imagingStudyBL = imagingStudyBL;
        _imagingStudyValidationService = imagingStudyValidationService;
        _csvFileValidator = csvFileValidator;
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
    [HttpGet("{id:long}")]
    public async Task<ActionResult<ImagingStudyResponse>> GetById(long id)
    {
        var imagingStudiesModel = new ImagingStudiesModel { ImagingStudyItem = new ImagingStudyItem { Id = id } };
        var result = await _imagingStudyBL.GetById(imagingStudiesModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.ImagingStudyResponse);
    }

    /// <summary>Updates an existing imaging study.</summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ImagingStudyResponse>> Update(long id, [FromBody] ImagingStudyRequest imagingStudyRequest)
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

    /// <summary>Bulk imports imaging studies from a CSV file (Synthea imaging_studies.csv format). Matching Patient and Encounter records must already exist.</summary>
    [HttpPost("import")]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ImportResult>> Import(IFormFile file)
    {
        var (isValid, errorMessage) = _csvFileValidator.Validate(file);
        if (!isValid)
            return BadRequest(errorMessage);

        var result = await _imagingStudyBL.Import(file.OpenReadStream());
        return Ok(result);
    }

    /// <summary>Bulk upserts imaging studies from a CSV file (Synthea imaging_studies.csv format). Rows are matched by InstanceUid: matches are updated in place, new InstanceUids are inserted. Matching Patient and Encounter records must already exist.</summary>
    [HttpPost("import/upsert")]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ImportResult>> ImportUpsert(IFormFile file)
    {
        var (isValid, errorMessage) = _csvFileValidator.Validate(file);
        if (!isValid)
            return BadRequest(errorMessage);

        var result = await _imagingStudyBL.ImportUpsert(file.OpenReadStream());
        return Ok(result);
    }

    /// <summary>Deletes an imaging study by Id.</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var imagingStudiesModel = new ImagingStudiesModel { ImagingStudyItem = new ImagingStudyItem { Id = id } };
        var result = await _imagingStudyBL.Delete(imagingStudiesModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
