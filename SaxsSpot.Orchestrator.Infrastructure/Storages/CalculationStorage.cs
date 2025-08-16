using Gridify;
using Gridify.EntityFramework;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Domain.Entities;
using SaxsSpot.Orchestrator.Infrastructure.DbContexts;

namespace SaxsSpot.Orchestrator.Infrastructure.Storages;

public class CalculationStorage(CalculationDbContext dbContext)
    : GenericStorage<Calculation>(dbContext), ICalculationStorage
{
    public Task<Paging<Calculation>> Gridify(GridifyQuery query)
    {
        return dbContext.Entities.GridifyAsync(query);
    }
}