using HC.AI.MAPI.BL.HelloWorld;
using Microsoft.AspNetCore.Mvc;

namespace HC.AI.MAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelloWorldController : ControllerBase
{
    private readonly IHelloWorldBL _helloWorldBL;

    public HelloWorldController(IHelloWorldBL helloWorldBL)
    {
        _helloWorldBL = helloWorldBL;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_helloWorldBL.GetGreeting());
    }
}
