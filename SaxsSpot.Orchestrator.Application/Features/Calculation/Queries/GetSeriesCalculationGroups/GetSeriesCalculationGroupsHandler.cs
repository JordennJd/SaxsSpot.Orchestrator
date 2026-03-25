using System.Security.Cryptography;
using System.Text;
using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Contracts.Models;
using SaxsSpot.Orchestrator.Domain.NanosystemApi;
using SaxsSpot.Shared.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.GetSeriesCalculationGroups;

public class GetSeriesCalculationGroupsHandler(
    INanosystemApi nanosystemApi,
    ICalculationStorage calculationStorage
) : IRequestHandler<GetSeriesCalculationGroupsRequest, IResult<List<SeriesCalculationGroupDto>>>
{
    private sealed record CalculationInputKey(
        double QVectorFrom,
        double QVectorTo,
        int QSpaceMethod,
        int QScaleMethod,
        double QSpaceParameter,
        double? PhiVectorFrom,
        double? PhiVectorTo,
        int? PhiSpaceMethod,
        int? PhiScaleMethod,
        double? PhiSpaceParameter,
        double? ThetaVectorFrom,
        double? ThetaVectorTo,
        int? ThetaSpaceMethod,
        int? ThetaScaleMethod,
        double? ThetaSpaceParameter
    );

    public async Task<IResult<List<SeriesCalculationGroupDto>>> Handle(
        GetSeriesCalculationGroupsRequest request,
        CancellationToken cancellationToken)
    {
        var nanosystems = await nanosystemApi.GetNanosystemsAsync(
            new ApiQuery($"seriesId={request.SeriesId}"),
            cancellationToken);

        var nanosystemIds = nanosystems.Select(x => x.Id).Distinct().ToList();
        if (nanosystemIds.Count == 0)
            return FluentResults.Result.Ok(new List<SeriesCalculationGroupDto>());

        var calculations = await calculationStorage.WhereAsync(c => nanosystemIds.Contains(c.NanosystemId));

        var grouped = calculations
            .GroupBy(c => new CalculationInputKey(
                c.QVectorFrom,
                c.QVectorTo,
                c.QSpaceMethod,
                c.QScaleMethod,
                c.QSpaceParameter,
                c.PhiVectorFrom,
                c.PhiVectorTo,
                c.PhiSpaceMethod,
                c.PhiScaleMethod,
                c.PhiSpaceParameter,
                c.ThetaVectorFrom,
                c.ThetaVectorTo,
                c.ThetaSpaceMethod,
                c.ThetaScaleMethod,
                c.ThetaSpaceParameter))
            .Select(g =>
            {
                // One calculation per nanosystem for this parameter set: take the freshest.
                var perSystem = g
                    .GroupBy(x => x.NanosystemId)
                    .Select(x => x
                        .OrderByDescending(c => c.CalculateEnd)
                        .ThenByDescending(c => c.CalculateStart)
                        .ThenByDescending(c => c.InputDate)
                        .First())
                    .ToList();

                var groupId = BuildGroupId(g.Key);

                return new SeriesCalculationGroupDto(
                    GroupId: groupId,
                    Parameters: new CalculationInputParametersDto(
                        g.Key.QVectorFrom,
                        g.Key.QVectorTo,
                        g.Key.QSpaceMethod,
                        g.Key.QScaleMethod,
                        g.Key.QSpaceParameter,
                        g.Key.PhiVectorFrom,
                        g.Key.PhiVectorTo,
                        g.Key.PhiSpaceMethod,
                        g.Key.PhiScaleMethod,
                        g.Key.PhiSpaceParameter,
                        g.Key.ThetaVectorFrom,
                        g.Key.ThetaVectorTo,
                        g.Key.ThetaSpaceMethod,
                        g.Key.ThetaScaleMethod,
                        g.Key.ThetaSpaceParameter
                    ),
                    SystemsCount: perSystem.Count,
                    CalculationIds: perSystem.Select(x => x.Id).ToList(),
                    NanosystemIds: perSystem.Select(x => x.NanosystemId).Distinct().ToList()
                );
            })
            .OrderByDescending(x => x.SystemsCount)
            .ThenBy(x => x.GroupId, StringComparer.Ordinal)
            .ToList();

        return FluentResults.Result.Ok(grouped);
    }

    private static string BuildGroupId(CalculationInputKey key)
    {
        // Stable short identifier for frontend selection.
        var s = System.FormattableString.Invariant(
            $"{key.QVectorFrom:G17}|{key.QVectorTo:G17}|{key.QSpaceMethod}|{key.QScaleMethod}|{key.QSpaceParameter:G17}|{key.PhiVectorFrom:G17}|{key.PhiVectorTo:G17}|{key.PhiSpaceMethod}|{key.PhiScaleMethod}|{key.PhiSpaceParameter:G17}|{key.ThetaVectorFrom:G17}|{key.ThetaVectorTo:G17}|{key.ThetaSpaceMethod}|{key.ThetaScaleMethod}|{key.ThetaSpaceParameter:G17}");
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(s));
        return Convert.ToHexString(bytes.AsSpan(0, 8)).ToLowerInvariant(); // 16 hex chars
    }
}

