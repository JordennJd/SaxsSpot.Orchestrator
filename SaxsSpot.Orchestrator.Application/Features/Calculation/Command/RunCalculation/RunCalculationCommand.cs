using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Messages;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Command.RunCalculation;

public record RunCalculationCommand : CalculateScatteringRequest, IRequest<IResult<Guid>>;