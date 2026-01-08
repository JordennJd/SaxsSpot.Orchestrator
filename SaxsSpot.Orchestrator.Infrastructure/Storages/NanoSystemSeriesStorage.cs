using Gridify;
using Gridify.EntityFramework;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Domain.Entities;
using SaxsSpot.Orchestrator.Infrastructure.DbContexts;

namespace SaxsSpot.Orchestrator.Infrastructure.Storages;

public class NanoSystemSeriesStorage(NanosystemSeriesDbContext dbContext)
    : GenericStorage<NanosystemSeries>(dbContext), INanoSystemSeriesStorage
{
    public Task<Paging<NanosystemSeries>> Gridify(GridifyQuery query)
    {
        return dbContext.Entities.GridifyAsync(query);
    }
}
