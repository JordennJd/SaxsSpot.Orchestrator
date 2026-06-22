using FluentResults;
using MediatR;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.DownloadCalculation;

public record DownloadCalculationQuery(Guid Id) : IRequest<IResult<Stream>>;
