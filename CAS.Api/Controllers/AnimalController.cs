using CAS.Core;
using Microsoft.AspNetCore.Mvc;

namespace CAS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalController(ILogger<AnimalController> logger, ICoreFacade facade) : ControllerBase
{
    [HttpGet("{id}", Name = "AnimalGet")]
    public async Task<IActionResult> GetAsync(string id)
    {
        return Ok(await facade.Animals.GetAsync(id));
    }
}
