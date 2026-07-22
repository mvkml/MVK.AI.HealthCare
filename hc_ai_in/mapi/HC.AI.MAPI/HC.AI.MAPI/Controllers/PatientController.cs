using HC.AI.MAPI.Models;
using HC.AI.MAPI.Services;
using HC.AI.MAPI.Tool.Validation;
using Microsoft.AspNetCore.Mvc;

namespace HC.AI.MAPI.Controllers;

/// <summary>
/// Entry point for the Patient persona's healthcare assistant (US010 real-backend follow-up,
/// PB024/PB034). Mirrors <see cref="DoctorController"/>'s locked <c>provide-prompt</c> shape —
/// only that one endpoint, since the Patient chat feature has no equivalent of Doctor's older
/// demo endpoints (Get/base-concept/chat-response-by-prompt).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly IPromptValidationUtility _promptValidationUtility;

    public PatientController(IPatientService patientService, IPromptValidationUtility promptValidationUtility)
    {
        _patientService = patientService;
        _promptValidationUtility = promptValidationUtility;
    }

    /// <summary>
    /// Validates and processes a structured prompt request for the Patient persona.
    /// </summary>
    /// <param name="request">The prompt request; at minimum, <see cref="PromptRequest.Message"/> is required.</param>
    /// <returns>The assistant's structured prompt response.</returns>
    /// <response code="200">The assistant responded successfully.</response>
    /// <response code="400">The request failed validation.</response>
    [HttpPost("provide-prompt")]
    [ProducesResponseType(typeof(PromptResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProvidePrompt([FromBody] PromptRequest request)
    {
        var validationResult = _promptValidationUtility.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var model = new PromptModel { PromptRequest = request };
        var response = await _patientService.ProvidePromptAsync(model);
        return Ok(response);
    }
}
