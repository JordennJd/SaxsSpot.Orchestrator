using AutoMapper;
using FluentResults;
using Gridify;
using MediatR;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.GetCalcualtionList;

public class GetCalculationListHandler(ICalculationStorage storage, IMapper mapper) : IRequestHandler<GetCalculationListQuery, IResult<Paging<CalculationDto>>>
{
    public async Task<IResult<Paging<CalculationDto>>> Handle(GetCalculationListQuery request, CancellationToken cancellationToken)
    {
        return FluentResults.Result.Ok(mapper.Map<Paging<CalculationDto>>(await storage.Gridify(request.Request)));
    }
}