using Microsoft.Extensions.Configuration;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.Orchestrator.Domain.Entities;

namespace SaxsSpot.Orchestrator.Infrastructure.DbContexts;

public class CalculationDbContext(IConfiguration configuration) : GenericDbContext<Calculation>(configuration);