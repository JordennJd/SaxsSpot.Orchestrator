using Gridify;
using SaxsSpot.Core.Contracts.Services;
using SaxsSpot.Orchestrator.Domain.Entities;

namespace SaxsSpot.Orchestrator.Application.Interfaces;

public interface INanoSystemSeriesStorage : IGenericStorage<NanosystemSeries>
{
    Task<Paging<NanosystemSeries>> Gridify(GridifyQuery query);
}
