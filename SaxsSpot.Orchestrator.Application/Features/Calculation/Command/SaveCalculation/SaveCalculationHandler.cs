using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Domain.Entities;
using SaxsSpot.Shared.ProgressTrackerClient.Contracts.Services;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Command;

public class SaveCalculationHandler(ILogger<SaveCalculationHandler> logger, IMapper mapper,
    ICalculationStorage storage, ICalculateObjectStorage objectStorage, IJobServiceClient jobServiceClient)
    : IRequestHandler<SaveCalculationCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(SaveCalculationCommand command, CancellationToken cancellationToken)
    {
        var operationId = command.Result.Request.RequestId;
        try
        {
            logger.LogInformation($"Calculation preparing to save to DB with operationID: {operationId}");
            var calculation = mapper.Map<Domain.Entities.Calculation>(command.Result.Request);
            var objectId = Guid.NewGuid();
            calculation.CalculateStart = command.Result.CalculateStart.ToUniversalTime();
            calculation.CalculateEnd = command.Result.CalculateEnd.ToUniversalTime();
            await objectStorage.Save(mapper.Map<IEnumerable<IntensityResult>>(command.Result.Result), objectId);
            calculation.ObjectId = objectId;
            await storage.UpdateOrInsertAsync(calculation);
            logger.LogInformation($"Calculation saved to DB with operationID: {operationId}");
            await jobServiceClient.CompleteJobAsync(new Shared.ProgressTrackerClient.Contracts.Models.
                CompleteJobQuery(command.Result.Request.RequestId, "Calculation complete successfully.", false));
            return FluentResults.Result.Ok(calculation.Id);
        }
        catch (Exception e)
        {
            logger.LogCritical($"Error saving calculation with Id: {operationId}, Exception: {e.Message}");
            throw;
        }
    }
}