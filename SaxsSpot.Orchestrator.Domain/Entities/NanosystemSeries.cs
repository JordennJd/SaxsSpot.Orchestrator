using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;

namespace SaxsSpot.Orchestrator.Domain.Entities;

/// <summary>
/// Nanosystem series entity - represents a series of nanosystem generations
/// </summary>
public class NanosystemSeries
{
    public Guid Id { get; set; }

    public ParticleKind ParticleKind { get; set; }

    public int ParticleCountFrom { get; set; }

    public int ParticleCountTo { get; set; }

    public double GlobalSizeFrom { get; set; }

    public double GlobalSizeTo { get; set; }

    public double NumericalConcentrationFrom { get; set; }

    public double NumericalConcentrationTo { get; set; }

    public double? ExcessFrom { get; set; }

    public double? ExcessTo { get; set; }

    public float MaxParticleSizeFrom { get; set; }

    public float MaxParticleSizeTo { get; set; }

    public float MinParticleSizeFrom { get; set; }

    public float MinParticleSizeTo { get; set; }

    public float KFrom { get; set; }

    public float KTo { get; set; }

    public float ThetaFrom { get; set; }

    public float ThetaTo { get; set; }
}
