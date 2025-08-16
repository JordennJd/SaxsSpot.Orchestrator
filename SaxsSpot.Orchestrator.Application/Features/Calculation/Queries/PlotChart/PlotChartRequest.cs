using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Enums;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.PlotChart;

public record PlotChartRequest(
    string ChartTitle,
    string XAxis,
    string YAxis,
    List<Guid> CalculatesId,
    SpaceMethod ScaleMethodsX,
    SpaceMethod ScaleMethodsY
) : IRequest<IResult<string>>;