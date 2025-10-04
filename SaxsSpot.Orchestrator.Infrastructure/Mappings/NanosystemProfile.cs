using AutoMapper;
using SaxsSpot.NanoSystemService.Contracts.Models;
using SaxsSpot.Orchestrator.Domain.Entities;

namespace SaxsSpot.Orchestrator.Infrastructure.Mappings;

public class NanosystemProfile : Profile
{
    public NanosystemProfile()
    {
        CreateMap<Nanosystem, NanosystemDto>()
            .ReverseMap();
    }
}