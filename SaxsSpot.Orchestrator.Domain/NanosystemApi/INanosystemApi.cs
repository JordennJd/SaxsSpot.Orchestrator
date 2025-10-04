using SaxsSpot.Orchestrator.Domain.Entities;
using SaxsSpot.Shared.Contracts.Models;

namespace SaxsSpot.Orchestrator.Domain.NanosystemApi;

public interface INanosystemApi
{
    public Task<IEnumerable<Nanosystem>> GetNanosystemsAsync(ApiQuery query, CancellationToken cancellation);
}