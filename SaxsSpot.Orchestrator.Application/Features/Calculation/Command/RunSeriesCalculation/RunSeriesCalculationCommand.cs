using System.Text.Json.Serialization;
using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Messages;
using SaxsSpot.Orchestrator.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Command.RunCalculation;

public record RunSeriesCalculationCommand : IRequest<IResult<Guid>>
{
    [JsonPropertyName("requestId")]
    public string? RequestId { get; set; }

    [JsonPropertyName("qVectorSpaceParameters")]
    public SpaceParameters QVectorSpaceParameters { get; init; }

    [JsonPropertyName("thetaVectorSpaceParameters")]
    public SpaceParameters ThetaVectorSpaceParameters { get; init; }

    [JsonPropertyName("phiVectorSpaceParameters")]
    public SpaceParameters PhiVectorSpaceParameters { get; init; }

    [JsonPropertyName("systemId")]
    public Guid SystemId { get; init; }
}