using AutoMapper;
using FluentResults;
using Gridify;
using MediatR;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.GetCalculation;

public class GetCalculationHandler(ICalculationStorage storage, IMapper mapper) : IRequestHandler<GetCalculationRequest, IResult<CalculationDto>>
{
    public async Task<IResult<CalculationDto>> Handle(GetCalculationRequest request, CancellationToken cancellationToken)
    {
        return FluentResults.Result.Ok(mapper.Map<CalculationDto>((
            await storage.Gridify(new GridifyQuery(1, 1, $"id={request.Id}")))
            .Data.FirstOrDefault()));
    }
}