using AutoMapper;
using FluentResults;
using Gridify;
using MediatR;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.GetCalculationListByNanosystemId;

public class GetCalculationByNanosystemIdHandler(ICalculationStorage storage, IMapper mapper) :
    IRequestHandler<GetCalculationByNanosystemIdRequest, IResult<Paging<CalculationDto>>>
{
    public async Task<IResult<Paging<CalculationDto>>> Handle(GetCalculationByNanosystemIdRequest request, CancellationToken cancellationToken)
    {
        var entities = await storage.Gridify(new GridifyQuery(request.CurrentPage, request.PageSize,
            $"nanosystemId={request.NanosystemId}"));
        
        return FluentResults.Result.Ok(mapper.Map<Paging<CalculationDto>>(entities));
    }
}