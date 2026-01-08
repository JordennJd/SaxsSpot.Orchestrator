using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaxsSpot.Orchestrator.Application.Features.Nanosystem.Command.RunMassGeneration;
using SaxsSpot.Orchestrator.Contracts.Messages;
using SaxsSpot.Shared.Contracts.Models;

namespace SaxsSpot.Orchestrator.Controllers;

[ApiController]
[Route("api/nanosystem")]
public class NanosystemController(IMediator mediator) : Controller
{
    [HttpPost("run-mass-generation")]
    public async Task<IActionResult> RunMassGeneration([FromBody] MassGenerateNanoSystemOptionsDto dto)
    {
        var result = await mediator.Send(new RunMassGenerationCommand(dto));
        if (!result.IsSuccess)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }
}
