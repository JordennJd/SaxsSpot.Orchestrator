using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Messages;

namespace SaxsSpot.Orchestrator.Application.Features.Nanosystem.Command.RunGeneration;

public record RunGenerationCommand : RunGenerationRequest, IRequest<IResult<Guid>>;
