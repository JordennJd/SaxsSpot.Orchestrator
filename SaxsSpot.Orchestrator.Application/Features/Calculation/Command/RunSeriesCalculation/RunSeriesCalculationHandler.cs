using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.Shared.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Command.RunSeriesCalculation;

using Exceptions;
using RunCalculation;
using Domain.NanosystemApi;

public class RunSeriesCalculationHandler(ILogger<RunSeriesCalculationHandler> logger,IMediator mediator, INanosystemApi nanosystemApi) : IRequestHandler<RunSeriesCalculationCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(RunSeriesCalculationCommand request, CancellationToken cancellationToken)
    {
        var operationId = Guid.NewGuid();
        logger.LogInformation("Starting calculation with id {id}", operationId);
        try
        {
            var nanosystems = await nanosystemApi.GetNanosystemsAsync(new ApiQuery($"seriesId={request.SystemId}"), cancellationToken);

            foreach (var nanosystem in nanosystems)
            {
                await mediator.Send(new RunCalculationCommand
                {
                    SystemId = nanosystem.Id,
                    ParticleKind = (ParticleKind)nanosystem.ParticleKind,
                    PhiVectorSpaceParameters = request.PhiVectorSpaceParameters,
                    ThetaVectorSpaceParameters = request.ThetaVectorSpaceParameters,
                    QVectorSpaceParameters = request.QVectorSpaceParameters,
                    RequestId = operationId.ToString()
                }, cancellationToken);
            }

            return FluentResults.Result.Ok(operationId);
        }
        catch (ApiCallException ex)
        {
            logger.LogInformation("Error during calculation start with id {id} error: {ex}", operationId, ex.ToString());
            return FluentResults.Result.Fail<Guid>($"Error during call {ex.ServiceName}");
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error during calculation start with id {id} error: {ex}", operationId, ex.ToString());
            return FluentResults.Result.Fail<Guid>($"Error during calculation start");
        }
    }
}