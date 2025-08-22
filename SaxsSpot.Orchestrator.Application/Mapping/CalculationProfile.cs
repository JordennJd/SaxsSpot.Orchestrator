using AutoMapper;
using Gridify;
using SaxsSpot.Orchestrator.Contracts.Messages;
using SaxsSpot.Orchestrator.Contracts.Models;
using SaxsSpot.Orchestrator.Domain.Entities;

namespace SaxsSpot.Orchestrator.Application.Mapping;

public class CalculationProfile : Profile
{
    public CalculationProfile()
    {
        CreateMap<CalculateScatteringRequest, Calculation>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.NanosystemId, opt => opt.MapFrom(src => src.SystemId))
            .ForMember(dest => dest.ObjectId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.QVectorFrom, opt => opt.MapFrom(src => src.QVectorSpaceParameters.Start))
            .ForMember(dest => dest.QVectorTo, opt => opt.MapFrom(src => src.QVectorSpaceParameters.End))
            .ForMember(dest => dest.QSpaceMethod, opt => opt.MapFrom(src => (int)src.QVectorSpaceParameters.SpaceMethod))
            .ForMember(dest => dest.QScaleMethod, opt => opt.MapFrom(src => (int)src.QVectorSpaceParameters.ScaleMethod))
            .ForMember(dest => dest.QSpaceParameter, opt => opt.MapFrom(src => src.QVectorSpaceParameters.SpaceParameter))
            .ForMember(dest => dest.PhiVectorFrom, opt => opt.MapFrom(src => src.PhiVectorSpaceParameters.Start))
            .ForMember(dest => dest.PhiVectorTo, opt => opt.MapFrom(src => src.PhiVectorSpaceParameters.End))
            .ForMember(dest => dest.PhiSpaceMethod, opt => opt.MapFrom(src => src.PhiVectorSpaceParameters.SpaceMethod))
            .ForMember(dest => dest.PhiScaleMethod, opt => opt.MapFrom(src => src.PhiVectorSpaceParameters.ScaleMethod))
            .ForMember(dest => dest.PhiSpaceParameter, opt => opt.MapFrom(src => src.PhiVectorSpaceParameters.SpaceParameter))
            .ForMember(dest => dest.ThetaVectorFrom, opt => opt.MapFrom(src => src.ThetaVectorSpaceParameters.Start))
            .ForMember(dest => dest.ThetaVectorTo, opt => opt.MapFrom(src => src.ThetaVectorSpaceParameters.End))
            .ForMember(dest => dest.ThetaSpaceMethod, opt => opt.MapFrom(src => (int)src.ThetaVectorSpaceParameters.SpaceMethod))
            .ForMember(dest => dest.ThetaScaleMethod, opt => opt.MapFrom(src => (int)src.ThetaVectorSpaceParameters.ScaleMethod))
            .ForMember(dest => dest.ThetaSpaceParameter, opt => opt.MapFrom(src => src.ThetaVectorSpaceParameters.SpaceParameter))
            .ForMember(dest => dest.InputDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.CalculateStart, opt => opt.Ignore())
            .ForMember(dest => dest.CalculateEnd, opt => opt.Ignore());
        
        CreateMap<Calculation, CalculationDto>();
        CreateMap<Paging<Calculation>, Paging<CalculationDto>>();
    }
}