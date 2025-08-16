using System.Text.Json.Serialization;
using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Messages;
using SaxsSpot.Orchestrator.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Command.RunCalculation;

public record RunSeriesCalculationCommand : IRequest<IResult<Guid>>
{
    [JsonPropertyName("qVectorSpaceParameters")]
    public SpaceParameters QVectorSpaceParameters { get; init; }

    [JsonPropertyName("thetaVectorSpaceParameters")]
    public SpaceParameters ThetaVectorSpaceParameters { get; init; }

    [JsonPropertyName("phiVectorSpaceParameters")]
    public SpaceParameters PhiVectorSpaceParameters { get; init; }

    [JsonPropertyName("seriesId")]
    public Guid SeriesId { get; init; }
}