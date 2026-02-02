using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Enums;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.BuildChartAnalyseAverage;

public record BuildChartAnalyseAverageRequest(
    Guid[] RadialAnalysisIds,
    string ChartTitle,
    string XAxis,
    string YAxis,
    SpaceMethod ScaleMethodsX,
    SpaceMethod ScaleMethodsY
) : IRequest<IResult<string>>;
