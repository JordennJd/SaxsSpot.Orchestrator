using Microsoft.Extensions.Configuration;
using SaxsSpot.Core.CommonObjectStorage.Engine;
using SaxsSpot.Core.Contracts.Attributes;
using SaxsSpot.Core.Contracts.Services;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Domain.Entities;

namespace SaxsSpot.Orchestrator.Infrastructure.Storages;

public class CalculateObjectStorage : CommonObjectStorage<IntensityResult>, ICalculateObjectStorage
{
    public CalculateObjectStorage(IConfiguration configuration) : base(configuration)
    {
    }

    protected override Stream GetStream(IEnumerable<IntensityResult> data)
    {
        var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);

        foreach (var particle in data)
        {
            streamWriter.WriteLine(particle.ToString());
        }

        streamWriter.Flush();
        return stream;    }

    protected override IEnumerable<IntensityResult> FromStream(Stream data)
    {
        using var streamReader = new StreamReader(data, leaveOpen: true);
        
        string? str;
        while ((str = streamReader.ReadLine()) != null)
        {
            var splitted = str.Split(' ');

            yield return new IntensityResult(float.Parse(splitted[0]), float.Parse(splitted[1]));
        }
    }
}