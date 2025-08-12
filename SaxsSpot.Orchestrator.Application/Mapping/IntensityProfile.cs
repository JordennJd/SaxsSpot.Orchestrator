using AutoMapper;
using SaxsSpot.Orchestrator.Contracts.Messages;

namespace SaxsSpot.Orchestrator.Application.Mapping;

public class IntensityProfile : Profile
{
    public IntensityProfile()
    {
        CreateMap<IntensityResult, Domain.Entities.IntensityResult>().ReverseMap();
    }
}