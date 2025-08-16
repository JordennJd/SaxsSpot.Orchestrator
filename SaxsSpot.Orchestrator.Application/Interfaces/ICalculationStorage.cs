using Gridify;
using SaxsSpot.Core.Contracts.Services;
using SaxsSpot.Orchestrator.Contracts.Models;
using SaxsSpot.Orchestrator.Domain.Entities;

namespace SaxsSpot.Orchestrator.Application.Interfaces;

public interface ICalculationStorage  : IGenericStorage<Calculation>
{
    Task<Paging<Calculation>> Gridify(GridifyQuery query);
}