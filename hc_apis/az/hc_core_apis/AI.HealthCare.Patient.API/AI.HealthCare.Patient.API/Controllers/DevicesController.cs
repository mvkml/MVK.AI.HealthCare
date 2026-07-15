using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Device;
using Microsoft.AspNetCore.Mvc;

namespace AI.HealthCare.Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceBL _deviceBL;
    private readonly IDeviceValidationService _deviceValidationService;

    public DevicesController(IDeviceBL deviceBL, IDeviceValidationService deviceValidationService)
    {
        _deviceBL = deviceBL;
        _deviceValidationService = deviceValidationService;
    }

    /// <summary>Creates a new device.</summary>
    [HttpPost]
    public async Task<ActionResult<DeviceResponse>> Create([FromBody] DeviceRequest deviceRequest)
    {
        var devicesModel = new DevicesModel { DeviceRequest = deviceRequest };

        devicesModel = _deviceValidationService.Validate(devicesModel);
        if (devicesModel.IsNotValid)
        {
            return BadRequest(new DeviceResponse
            {
                IsNotValid = true,
                Message = devicesModel.Message,
            });
        }

        devicesModel = await _deviceBL.Create(devicesModel);
        return Ok(devicesModel.DeviceResponse);
    }

    /// <summary>Returns all devices.</summary>
    [HttpGet]
    public async Task<ActionResult<List<DeviceItem>>> GetAll()
    {
        var result = await _deviceBL.GetAll(new DevicesModel());
        return Ok(result.DeviceItems);
    }

    /// <summary>Returns all devices for a given patient.</summary>
    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<List<DeviceItem>>> GetByPatientId(Guid patientId)
    {
        var result = await _deviceBL.GetByPatientId(patientId);
        return Ok(result.DeviceItems);
    }

    /// <summary>Returns a single device by Id.</summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<DeviceResponse>> GetById(long id)
    {
        var devicesModel = new DevicesModel { DeviceItem = new DeviceItem { Id = id } };
        var result = await _deviceBL.GetById(devicesModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(result.DeviceResponse);
    }

    /// <summary>Updates an existing device.</summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<DeviceResponse>> Update(long id, [FromBody] DeviceRequest deviceRequest)
    {
        var devicesModel = new DevicesModel
        {
            DeviceRequest = deviceRequest,
            DeviceItem = new DeviceItem { Id = id }
        };

        devicesModel = _deviceValidationService.Validate(devicesModel);
        if (devicesModel.IsNotValid)
        {
            return BadRequest(new DeviceResponse
            {
                IsNotValid = true,
                Message = devicesModel.Message,
            });
        }

        devicesModel = await _deviceBL.Update(devicesModel);
        if (devicesModel.IsNotValid)
            return NotFound(devicesModel.Message);

        return Ok(devicesModel.DeviceResponse);
    }

    /// <summary>Deletes a device by Id.</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var devicesModel = new DevicesModel { DeviceItem = new DeviceItem { Id = id } };
        var result = await _deviceBL.Delete(devicesModel);
        if (result.IsNotValid)
            return NotFound(result.Message);

        return Ok(new { result.Message });
    }
}
