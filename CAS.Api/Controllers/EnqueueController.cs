using CAS.Core.Models;
using CAS.Infrastructure.Queues;
using Microsoft.AspNetCore.Mvc;

namespace CAS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnqueueController(ILogger<EnqueueController> logger, IQueuePublisher<ExampleModel> publisher) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostAsync(ExampleModel payload)
    {
        return Ok(await publisher.PublishMessage(payload));
    }
}