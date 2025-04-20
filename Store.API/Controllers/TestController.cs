using Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Store.API.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult ThrowException()
    {
        throw new Exception("This is a test exception from controller.");
    }

    [HttpGet]
    public IActionResult DivideException()
    {
        throw new DivideByZeroException("This is a test exception from controller.");
    }

    [HttpGet]
    public IActionResult UnauthorizedAccessException()
    {
        throw new UnauthorizedAccessException("This is a test exception from controller.");
    }
    
    [HttpGet]
    public IActionResult NotFoundException()
    {
        throw new NotFoundException("This is a test exception from controller.");
    }
}