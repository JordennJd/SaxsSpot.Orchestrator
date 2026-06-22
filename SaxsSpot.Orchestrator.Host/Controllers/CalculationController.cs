using Gridify;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Command.RunCalculation;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Command.RunSeriesCalculation;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.BuildChartAnalyse;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.BuildChartAnalyseAverage;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.BuildChartAnalysePng;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.BuildChartAnalyseAveragePng;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.GetCalcualtionList;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.GetCalculation;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.GetCalculationListByNanosystemId;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.GetSeriesCalculationGroups;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.PlotChart;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.PlotChartAverage;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.PlotChartPng;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.PlotChartAveragePng;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.PlotScatteringCompare;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.PlotScatteringComparePng;
using SaxsSpot.Shared.Contracts.Models;

namespace SaxsSpot.Orchestrator.Controllers;

[ApiController]
[Route("api/calculation")]
public class CalculationController(IMediator mediator) : Controller
{
    [HttpPost("run-calculation")]
    public async Task<IActionResult> RunGeneration([FromBody] RunCalculationCommand dto)
    {
        var result = await mediator.Send(dto);
        return Ok(result.ToResultDto());
    }
    
    [HttpPost("run-series-calculation")]
    public async Task<IActionResult> RunSeriesGeneration([FromBody] RunSeriesCalculationCommand dto)
    {
        var result = await mediator.Send(dto);
        return Ok(result.ToResultDto());
    }
    
    [HttpGet("list")]
    public async Task<IActionResult> GetCalculationList([FromQuery] GridifyQuery dto)
    {
        var result = await mediator.Send(new GetCalculationListQuery(dto));
        if (!result.IsSuccess)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }
    
    [HttpGet("list-by-nanosystem")]
    public async Task<IActionResult> GetCalculationListByNanosystemId([FromQuery] GetCalculationByNanosystemIdRequest dto)
    {
        var result = await mediator.Send(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }

    [HttpGet("series-groups")]
    public async Task<IActionResult> GetSeriesCalculationGroups([FromQuery] Guid seriesId)
    {
        var result = await mediator.Send(new GetSeriesCalculationGroupsRequest(seriesId));
        if (!result.IsSuccess)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCalculation([FromQuery] GetCalculationRequest dto)
    {
        var result = await mediator.Send(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }
    
    [HttpGet("plot")]
    public async Task<IActionResult> BuildChart([FromQuery] PlotChartRequest dto)
    {
        var result = await mediator.Send(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }

    [HttpGet("plot-png")]
    public async Task<IActionResult> BuildChartPng([FromQuery] PlotChartPngRequest dto)
    {
        var result = await mediator.Send(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }

    [HttpGet("plot-chart-average")]
    public async Task<IActionResult> BuildChartAverage([FromQuery] PlotChartAverageRequest dto)
    {
        var result = await mediator.Send(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }

    [HttpGet("plot-chart-average-png")]
    public async Task<IActionResult> BuildChartAveragePng([FromQuery] PlotChartAveragePngRequest dto)
    {
        var result = await mediator.Send(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }

    [HttpGet("plot-analyse")]
    public async Task<IActionResult> BuildChartAnalyse([FromQuery] BuildChartAnalyseRequest dto)
    {
        var result = await mediator.Send(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }

    [HttpGet("plot-analyse-png")]
    public async Task<IActionResult> BuildChartAnalysePng([FromQuery] BuildChartAnalysePngRequest dto)
    {
        var result = await mediator.Send(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }

    [HttpGet("plot-analyse-average")]
    public async Task<IActionResult> BuildChartAnalyseAverage([FromQuery] BuildChartAnalyseAverageRequest dto)
    {
        var result = await mediator.Send(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }

    [HttpGet("plot-analyse-average-png")]
    public async Task<IActionResult> BuildChartAnalyseAveragePng([FromQuery] BuildChartAnalyseAveragePngRequest dto)
    {
        var result = await mediator.Send(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }

    [HttpGet("plot-scattering-compare")]
    public async Task<IActionResult> PlotScatteringCompare([FromQuery] PlotScatteringCompareRequest dto)
    {
        var result = await mediator.Send(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }

    [HttpGet("plot-scattering-compare-png")]
    public async Task<IActionResult> PlotScatteringComparePng([FromQuery] PlotScatteringComparePngRequest dto)
    {
        var result = await mediator.Send(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }
}