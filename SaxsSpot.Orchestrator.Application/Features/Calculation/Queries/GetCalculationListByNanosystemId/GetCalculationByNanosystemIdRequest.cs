using FluentResults;
using Gridify;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.GetCalculationListByNanosystemId;

public record GetCalculationByNanosystemIdRequest(Guid NanosystemId, int CurrentPage, int PageSize)
    : IRequest<IResult<Paging<CalculationDto>>>;