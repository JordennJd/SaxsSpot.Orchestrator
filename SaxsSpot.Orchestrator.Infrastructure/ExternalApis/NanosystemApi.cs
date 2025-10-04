using AutoMapper;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Sdk.Interfaces;
using SaxsSpot.Orchestrator.Application.Exceptions;
using SaxsSpot.Orchestrator.Domain.Entities;
using SaxsSpot.Orchestrator.Domain.NanosystemApi;
using SaxsSpot.Shared.Contracts.Models;

namespace SaxsSpot.Orchestrator.Infrastructure.ExternalApis;

public class NanosystemApi(INanosystemServiceApiClient client, IMapper mapper, ILogger<NanosystemApi> logger) : INanosystemApi
{
    private const string ServiceName = "nanosystem-service";

    public async Task<IEnumerable<Nanosystem>> GetNanosystemsAsync(ApiQuery query, CancellationToken cancellation)
    {
        try
        {
            var result = await client.GetNanosystemList(query, cancellation);
            var domain = mapper.Map<IEnumerable<Nanosystem>>(result.Result);
            return domain;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw new ApiCallException(ServiceName, ex);
        }
    }
}