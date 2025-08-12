using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Domain.Entities;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Command;

public class SaveCalculationHandler(ILogger<SaveCalculationHandler> logger, IMapper mapper,
    ICalculationStorage storage, ICalculateObjectStorage objectStorage)
    : IRequestHandler<SaveCalculationCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(SaveCalculationCommand command, CancellationToken cancellationToken)
    {
        var operationId = command.result.Request.RequestId;
        try
        {
            logger.LogInformation($"Calculation preparing to save to DB with operationID: {operationId}");
            var calculation = mapper.Map<Domain.Entities.Calculation>(command.result.Request);
            var objectId = Guid.NewGuid();
            await objectStorage.Save(mapper.Map<IEnumerable<IntensityResult>>(command.result.Result), objectId);
            calculation.ObjectId = objectId;
            await storage.UpdateOrInsertAsync(calculation);
            logger.LogInformation($"Calculation saved to DB with operationID: {operationId}");

            return FluentResults.Result.Ok(calculation.Id);
        }
        catch (Exception e)
        {
            logger.LogCritical($"Error saving calculation with Id: {operationId}, Exception: {e.Message}");
            throw;
        }
    }
}