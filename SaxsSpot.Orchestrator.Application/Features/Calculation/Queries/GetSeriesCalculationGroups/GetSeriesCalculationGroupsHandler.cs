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
    private sealed record QRangeKey(double QVectorFrom, double QVectorTo);

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

        var grouped = new List<SeriesCalculationGroupDto>();

        var parameterGroups = calculations
            .GroupBy(c => new QRangeKey(c.QVectorFrom, c.QVectorTo));

        foreach (var g in parameterGroups)
        {
            var perSystem = g
                .GroupBy(x => x.NanosystemId)
                .Select(x => x
                    .OrderByDescending(c => c.CalculateEnd)
                    .ThenByDescending(c => c.CalculateStart)
                    .ThenByDescending(c => c.InputDate)
                    .First())
                .ToList();

            var sample = perSystem[0];
            var groupId = BuildGroupId(g.Key);

            grouped.Add(new SeriesCalculationGroupDto(
                GroupId: groupId,
                Parameters: new CalculationInputParametersDto(
                    g.Key.QVectorFrom,
                    g.Key.QVectorTo,
                    sample.QSpaceMethod,
                    sample.QScaleMethod,
                    sample.QSpaceParameter,
                    sample.PhiVectorFrom,
                    sample.PhiVectorTo,
                    sample.PhiSpaceMethod,
                    sample.PhiScaleMethod,
                    sample.PhiSpaceParameter,
                    sample.ThetaVectorFrom,
                    sample.ThetaVectorTo,
                    sample.ThetaSpaceMethod,
                    sample.ThetaScaleMethod,
                    sample.ThetaSpaceParameter
                ),
                SystemsCount: perSystem.Count,
                CalculationIds: perSystem.Select(x => x.Id).ToList(),
                NanosystemIds: perSystem.Select(x => x.NanosystemId).Distinct().ToList()
            ));
        }

        grouped = grouped
            .OrderByDescending(x => x.SystemsCount)
            .ThenBy(x => x.GroupId, StringComparer.Ordinal)
            .ToList();

        return FluentResults.Result.Ok(grouped);
    }

    private static string BuildGroupId(QRangeKey key)
    {
        var s = System.FormattableString.Invariant($"{key.QVectorFrom:G17}|{key.QVectorTo:G17}");
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(s));
        return Convert.ToHexString(bytes.AsSpan(0, 8)).ToLowerInvariant();
    }
}
