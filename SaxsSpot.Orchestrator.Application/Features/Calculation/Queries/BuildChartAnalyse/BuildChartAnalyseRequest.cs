using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Enums;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.BuildChartAnalyse;

public record BuildChartAnalyseRequest(
    Guid[] RadialAnalysisIds,
    string ChartTitle,
    string XAxis,
    string YAxis,
    SpaceMethod ScaleMethodsX,
    SpaceMethod ScaleMethodsY
) : IRequest<IResult<string>>;

