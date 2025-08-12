using FluentResults;
using MassTransit;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Messages;
using SaxsSpot.Shared.ProgressTrackerClient.Contracts.Services;
using JobModels = SaxsSpot.Shared.ProgressTrackerClient.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Command.RunCalculation;

public class RunCalculationHandler(ITopicProducer<CalculateScatteringRequest> producer, IJobServiceClient jobServiceClient) : IRequestHandler<RunCalculationCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(RunCalculationCommand request, CancellationToken cancellationToken)
    {
        var operationId = Guid.NewGuid();
        request.RequestId = operationId.ToString();

        await producer.Produce(request, cancellationToken);
        await jobServiceClient.CreateJobAsync
            (new JobModels.CreateJobQuery(operationId.ToString(),
                "calculate-scattering", "calculate scatteing message produced by the SaxsSpot", ""));
        
        return FluentResults.Result.Ok(operationId);
    }
}