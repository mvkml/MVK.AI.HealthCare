using HC.AI.MAPI.BL.Persona;
using Microsoft.AspNetCore.Mvc;

namespace HC.AI.MAPI.Controllers;

/// <summary>
/// Demo/verification endpoint for the EPIC001 mock resolution mechanism (US011-US013,
/// implemented against <see cref="PersonaLlmConfigMockProvider"/> — see BACKLOG PB032, TASK017).
/// Not part of any locked/live feature; not called by aihcweb. Exists so the
/// Classification -> Executor resolution chain can be exercised and checked without wiring
/// changes into the live Doctor `provide-prompt` endpoint, since the fallback-behavior open
/// questions in US012/US013's acceptance criteria still need Product Owner sign-off.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PersonaModelResolutionController : ControllerBase
{
    private readonly IPersonaModelResolutionBL _resolutionBL;

    public PersonaModelResolutionController(IPersonaModelResolutionBL resolutionBL)
    {
        _resolutionBL = resolutionBL;
    }

    /// <summary>US012: resolve the Classification model + prompt for a persona (by RoleId).</summary>
    [HttpGet("classification/{roleId:int}")]
    public IActionResult GetClassification(int roleId)
    {
        var result = _resolutionBL.ResolveClassification(roleId);
        return result.IsResolved ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// US013: resolve the Executor model + prompt for a persona (by RoleId) + the prompt-type
    /// code a Classification run would have produced.
    /// </summary>
    [HttpGet("executor/{roleId:int}/{promptTypeCode}")]
    public IActionResult GetExecutor(int roleId, string promptTypeCode)
    {
        var result = _resolutionBL.ResolveExecutor(roleId, promptTypeCode);
        return result.IsResolved ? Ok(result) : NotFound(result);
    }
}
