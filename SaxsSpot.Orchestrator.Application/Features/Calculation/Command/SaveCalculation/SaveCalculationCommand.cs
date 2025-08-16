using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Messages;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Command;

public record SaveCalculationCommand(ScatteringResult Result) : IRequest<IResult<Guid>>;