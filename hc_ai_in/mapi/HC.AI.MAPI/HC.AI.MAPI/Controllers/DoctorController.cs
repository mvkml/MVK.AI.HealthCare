using HC.AI.MAPI.Models;
using HC.AI.MAPI.Services;
using HC.AI.MAPI.Tool.Validation;
using Microsoft.AspNetCore.Mvc;

namespace HC.AI.MAPI.Controllers;

/// <summary>
/// Entry point for the Doctor persona's natural-language healthcare assistant (US007).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    private readonly IPromptValidationUtility _promptValidationUtility;

    public DoctorController(IDoctorService doctorService, IPromptValidationUtility promptValidationUtility)
    {
        _doctorService = doctorService;
        _promptValidationUtility = promptValidationUtility;
    }

    /// <summary>
    /// Sends a doctor's message to the healthcare assistant and returns its response.
    /// </summary>
    /// <param name="message">The doctor's natural-language message, e.g. "hi".</param>
    /// <returns>The assistant's response text.</returns>
    /// <response code="200">The assistant responded successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get([FromQuery] string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return BadRequest("message is required.");
        }

        var response = await _doctorService.HandleRequestAsync(message);
        return Ok(response);
    }


    [HttpGet("base-concept")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBaseConcept([FromQuery] string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return BadRequest("message is required.");
        }

        var response = await _doctorService.BasicHandleRequestAsync(message);
        return Ok(response);
    }

     [HttpGet("chat-response-by-prompt")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetChatResponseByPrompt([FromQuery] string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return BadRequest("message is required.");
        }

        var response = await _doctorService.GetChatResponseByPrompt(message);
        return Ok(response);
    }

    /// <summary>
    /// Validates and processes a structured prompt request for the Doctor persona.
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
        var response = await _doctorService.ProvidePromptAsync(model);
        return Ok(response);
    }
}
