using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Enums;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.PlotChartAverage;

public record PlotChartAverageRequest(
    string ChartTitle,
    string XAxis,
    string YAxis,
    List<Guid> CalculatesId,
    SpaceMethod ScaleMethodsX,
    SpaceMethod ScaleMethodsY
) : IRequest<IResult<string>>;
