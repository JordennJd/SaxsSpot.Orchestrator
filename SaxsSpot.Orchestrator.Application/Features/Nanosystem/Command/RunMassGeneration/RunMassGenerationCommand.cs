using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Messages;

namespace SaxsSpot.Orchestrator.Application.Features.Nanosystem.Command.RunMassGeneration;

public record RunMassGenerationCommand(MassGenerateNanoSystemOptionsDto Parameters) : IRequest<IResult<Guid>>;
