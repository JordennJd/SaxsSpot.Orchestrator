using System.Text.Json.Serialization;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;

namespace SaxsSpot.Orchestrator.Contracts.Messages;

/// <summary>
/// Mass generation options for nanosystems
/// </summary>
public record MassGenerateNanoSystemOptionsDto
{
    [JsonPropertyName("options")]
    public IList<CommonParticleGenerationParametersDto> Options { get; init; } = new List<CommonParticleGenerationParametersDto>();
    
    [JsonPropertyName("nanoSystemsKind")]
    public ParticleKind NanoSystemsKind { get; init; }
}
