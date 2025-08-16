using FluentResults;
using Gridify;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.GetCalcualtionList;

public record GetCalculationListQuery(GridifyQuery Request) : IRequest<IResult<Paging<CalculationDto>>>;