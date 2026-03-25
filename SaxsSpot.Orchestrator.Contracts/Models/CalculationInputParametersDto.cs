namespace SaxsSpot.Orchestrator.Contracts.Models;

public record CalculationInputParametersDto(
    double QVectorFrom,
    double QVectorTo,
    int QSpaceMethod,
    int QScaleMethod,
    double QSpaceParameter,
    double? PhiVectorFrom,
    double? PhiVectorTo,
    int? PhiSpaceMethod,
    int? PhiScaleMethod,
    double? PhiSpaceParameter,
    double? ThetaVectorFrom,
    double? ThetaVectorTo,
    int? ThetaSpaceMethod,
    int? ThetaScaleMethod,
    double? ThetaSpaceParameter
);

