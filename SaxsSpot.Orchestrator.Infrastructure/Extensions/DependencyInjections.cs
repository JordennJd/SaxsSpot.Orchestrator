using Microsoft.Extensions.DependencyInjection;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Infrastructure.DbContexts;
using SaxsSpot.Orchestrator.Infrastructure.Storages;

namespace SaxsSpot.Orchestrator.Infrastructure.Extensions;

public static class DependencyInjections
{
    public static IServiceCollection AddCalculationServiceStorage(this IServiceCollection services)
    {
        return services.AddDbContext<CalculationDbContext>()
            .AddScoped<ICalculationStorage, CalculationStorage>()
            .AddScoped<ICalculateObjectStorage, CalculateObjectStorage>();
    }
}