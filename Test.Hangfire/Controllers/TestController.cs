using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Test.Hangfire.Models;
using Test.Hangfire.Services;

namespace Test.Hangfire.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private static List<Driver> drivers = new List<Driver>();

    private readonly ILogger<TestController> _logger;
    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult AddDriver(Driver driver)
    {
        if(ModelState.IsValid)
        {
            drivers.Add(driver);

            BackgroundJob.Enqueue<IServiceManagement>(x => x.UpdateDatabase());

            return CreatedAtAction("GetDriver", new { driver.Id }, driver);
        }

        return BadRequest();
    }

    [HttpGet]
    public IActionResult GetDriver(Guid id)
    {
        var driver = drivers.FirstOrDefault(x => x.Id == id);

        if (driver == null)
            return NotFound();

        return Ok(driver);
    }

    [HttpDelete]
    public IActionResult DeleteDriver(Guid id)
    {
        var driver = drivers.FirstOrDefault(x => x.Id == id);

        if (driver == null)
            return NotFound();

        driver.Status = 0;

        return NoContent();
    }
}
