using AI.HealthCare.Patient.API.Shared;
using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Organization;
using AI.HealthCare.Patient.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationBL _organizationBL;
    private readonly IOrganizationValidationService _organizationValidationService;
    private readonly ICsvFileValidator _csvFileValidator;

    public OrganizationsController(IOrganizationBL organizationBL, IOrganizationValidationService organizationValidationService, ICsvFileValidator csvFileValidator)
    {
        _organizationBL = organizationBL;
        _organizationValidationService = organizationValidationService;
        _csvFileValidator = csvFileValidator;
    }

    /// <summary>Creates a new organization.</summary>
    [HttpPost]
    public async Task<ActionResult<OrganizationResponse>> Create([FromBody] OrganizationRequest organizationRequest)
    {
        var organizationsModel = new OrganizationsModel { OrganizationRequest = organizationRequest };

        organizationsModel = _organizationValidationService.Validate(organizationsModel);
        if (organizationsModel.IsNotValid)
        {
            return BadRequest(new OrganizationResponse
            {
                IsNotValid = true,
                Message = organizationsModel.Message,
            });
        }

        organizationsModel = await _organizationBL.Create(organizationsModel);
        return Ok(organizationsModel.OrganizationResponse);
    }

    /// <summary>Returns all organizations.</summary>
    [HttpGet]
    public async Task<ActionResult<List<OrganizationItem>>> GetAll()
    {
        var result = await _organizationBL.GetAll(new OrganizationsModel());
        return Ok(result.OrganizationItems);
    }

    /// <summary>Returns a single organization by Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrganizationResponse>> GetById(Guid id)
    {
        var organizationsModel = new OrganizationsModel { OrganizationItem = new OrganizationItem { Id = id } };
        var result = await _organizationBL.GetById(organizationsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.OrganizationResponse);
    }

    /// <summary>Updates an existing organization.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<OrganizationResponse>> Update(Guid id, [FromBody] OrganizationRequest organizationRequest)
    {
        var organizationsModel = new OrganizationsModel
        {
            OrganizationRequest = organizationRequest,
            OrganizationItem = new OrganizationItem { Id = id }
        };

        organizationsModel = _organizationValidationService.Validate(organizationsModel);
        if (organizationsModel.IsNotValid)
        {
            return BadRequest(new OrganizationResponse
            {
                IsNotValid = true,
                Message = organizationsModel.Message,
            });
        }

        organizationsModel = await _organizationBL.Update(organizationsModel);
        if (organizationsModel.IsNotValid)
            return NotFound(organizationsModel.Message);

        return Ok(organizationsModel.OrganizationResponse);
    }

    /// <summary>Bulk imports organizations from a CSV file (Synthea organizations.csv format).</summary>
    [HttpPost("import")]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ImportResult>> Import(IFormFile file)
    {
        var (isValid, errorMessage) = _csvFileValidator.Validate(file);
        if (!isValid)
            return BadRequest(errorMessage);

        var result = await _organizationBL.Import(file.OpenReadStream());
        return Ok(result);
    }

    /// <summary>Bulk upserts organizations from a CSV file (Synthea organizations.csv format). Rows whose Id already exists are updated in place; new Ids are inserted.</summary>
    [HttpPost("import/upsert")]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ImportResult>> ImportUpsert(IFormFile file)
    {
        var (isValid, errorMessage) = _csvFileValidator.Validate(file);
        if (!isValid)
            return BadRequest(errorMessage);

        var result = await _organizationBL.ImportUpsert(file.OpenReadStream());
        return Ok(result);
    }

    /// <summary>Deletes an organization by Id.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var organizationsModel = new OrganizationsModel { OrganizationItem = new OrganizationItem { Id = id } };
        var result = await _organizationBL.Delete(organizationsModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
