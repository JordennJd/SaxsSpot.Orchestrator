using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Messages;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Command;

public record SaveCalculationCommand(ScatteringResult result) : IRequest<IResult<Guid>>;