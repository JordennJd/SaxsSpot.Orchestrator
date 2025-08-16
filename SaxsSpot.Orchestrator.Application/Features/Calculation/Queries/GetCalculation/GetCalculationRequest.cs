using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.GetCalculation;

public record GetCalculationRequest(Guid Id) : IRequest<IResult<CalculationDto>>;