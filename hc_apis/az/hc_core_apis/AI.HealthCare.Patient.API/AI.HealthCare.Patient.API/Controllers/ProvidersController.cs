using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Provider;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProvidersController : ControllerBase
{
    private readonly IProviderBL _providerBL;
    private readonly IProviderValidationService _providerValidationService;

    public ProvidersController(IProviderBL providerBL, IProviderValidationService providerValidationService)
    {
        _providerBL = providerBL;
        _providerValidationService = providerValidationService;
    }

    /// <summary>Creates a new provider.</summary>
    [HttpPost]
    public async Task<ActionResult<ProviderResponse>> Create([FromBody] ProviderRequest providerRequest)
    {
        var providersModel = new ProvidersModel { ProviderRequest = providerRequest };

        providersModel = _providerValidationService.Validate(providersModel);
        if (providersModel.IsNotValid)
        {
            return BadRequest(new ProviderResponse
            {
                IsNotValid = true,
                Message = providersModel.Message,
            });
        }

        providersModel = await _providerBL.Create(providersModel);
        return Ok(providersModel.ProviderResponse);
    }

    /// <summary>Returns all providers.</summary>
    [HttpGet]
    public async Task<ActionResult<List<ProviderItem>>> GetAll()
    {
        var result = await _providerBL.GetAll(new ProvidersModel());
        return Ok(result.ProviderItems);
    }

    /// <summary>Returns a single provider by Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProviderResponse>> GetById(Guid id)
    {
        var providersModel = new ProvidersModel { ProviderItem = new ProviderItem { Id = id } };
        var result = await _providerBL.GetById(providersModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.ProviderResponse);
    }

    /// <summary>Updates an existing provider.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProviderResponse>> Update(Guid id, [FromBody] ProviderRequest providerRequest)
    {
        var providersModel = new ProvidersModel
        {
            ProviderRequest = providerRequest,
            ProviderItem = new ProviderItem { Id = id }
        };

        providersModel = _providerValidationService.Validate(providersModel);
        if (providersModel.IsNotValid)
        {
            return BadRequest(new ProviderResponse
            {
                IsNotValid = true,
                Message = providersModel.Message,
            });
        }

        providersModel = await _providerBL.Update(providersModel);
        if (providersModel.IsNotValid)
            return NotFound(providersModel.Message);

        return Ok(providersModel.ProviderResponse);
    }

    /// <summary>Deletes a provider by Id.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var providersModel = new ProvidersModel { ProviderItem = new ProviderItem { Id = id } };
        var result = await _providerBL.Delete(providersModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
