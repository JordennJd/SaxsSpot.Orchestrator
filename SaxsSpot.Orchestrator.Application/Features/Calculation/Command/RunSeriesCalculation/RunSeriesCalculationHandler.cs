using FluentResults;
using MassTransit;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Messages;
using SaxsSpot.Shared.ProgressTrackerClient.Contracts.Services;
using JobModels = SaxsSpot.Shared.ProgressTrackerClient.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Command.RunCalculation;

public class RunSeriesCalculationHandler(IMediator mediator) : IRequestHandler<RunSeriesCalculationCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(RunSeriesCalculationCommand request, CancellationToken cancellationToken)
    {
        // var nanosystemsIds = 
        return FluentResults.Result.Ok<Guid>(new Guid());
    }
}