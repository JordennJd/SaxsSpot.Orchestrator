using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Command.RunCalculation;

namespace SaxsSpot.Orchestrator.Controllers;

[ApiController]
[Route("api/calculation")]
public class CalculationController(IMediator mediator) : Controller
{
    [HttpPost("run-calculation")]
    public async Task<IActionResult> RunGeneration([FromBody] RunCalculationCommand dto)
    {
        var result = await mediator.Send(dto);
        return Ok(result.ValueOrDefault);
    }
}