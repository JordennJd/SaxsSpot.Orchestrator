using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Enums;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.BuildChartAnalyseAveragePng;

public record BuildChartAnalyseAveragePngRequest(
    Guid[] RadialAnalysisIds,
    string ChartTitle,
    string XAxis,
    string YAxis,
    SpaceMethod ScaleMethodsX,
    SpaceMethod ScaleMethodsY
) : IRequest<IResult<string>>;

