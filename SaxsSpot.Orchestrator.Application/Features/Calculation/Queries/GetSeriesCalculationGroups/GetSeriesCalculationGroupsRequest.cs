using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.GetSeriesCalculationGroups;

public record GetSeriesCalculationGroupsRequest(Guid SeriesId)
    : IRequest<IResult<List<SeriesCalculationGroupDto>>>;

