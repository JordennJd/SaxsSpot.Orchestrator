using SaxsSpot.Core.Contracts.Attributes;
using SaxsSpot.Core.Contracts.Services;
using SaxsSpot.Orchestrator.Domain.Entities;

namespace SaxsSpot.Orchestrator.Application.Interfaces;

[SaxsService]
public interface ICalculateObjectStorage : ICommonObjectStorage<IntensityResult>
{
    
}