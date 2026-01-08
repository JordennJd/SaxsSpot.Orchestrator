using Microsoft.Extensions.Configuration;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.Orchestrator.Domain.Entities;

namespace SaxsSpot.Orchestrator.Infrastructure.DbContexts;

public class NanosystemSeriesDbContext(IConfiguration configuration) : GenericDbContext<NanosystemSeries>(configuration);
