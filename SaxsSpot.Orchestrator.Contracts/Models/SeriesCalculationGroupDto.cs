namespace SaxsSpot.Orchestrator.Contracts.Models;

public record SeriesCalculationGroupDto(
    string GroupId,
    CalculationInputParametersDto Parameters,
    int SystemsCount,
    List<Guid> CalculationIds,
    List<Guid> NanosystemIds
);

