namespace SaxsSpot.Orchestrator.Contracts.Models;

public record CalculationDto(
    Guid Id,
    Guid NanosystemId,
    Guid ObjectId,
    Guid UserId,
    float QVectorFrom,
    float QVectorTo,
    int QSpaceMethod,
    int QScaleMethod,
    float QSpaceParameter,
    float? PhiVectorFrom,
    float? PhiVectorTo,
    int? PhiSpaceMethod,
    int? PhiScaleMethod,
    float? PhiSpaceParameter,
    float? ThetaVectorFrom,
    float? ThetaVectorTo,
    int? ThetaSpaceMethod,
    int? ThetaScaleMethod,
    float? ThetaSpaceParameter,
    DateTime InputDate,
    DateTime CalculateStart,
    DateTime CalculateEnd
);