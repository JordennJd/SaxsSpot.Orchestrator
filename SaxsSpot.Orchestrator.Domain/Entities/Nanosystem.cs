namespace SaxsSpot.Orchestrator.Domain.Entities;

/// <summary>
/// Entity From NanosystemService
/// </summary>
public class Nanosystem
{
    public Guid Id { get; set; }
    
    public int ParticleKind { get; set; }
    
    public Guid SeriesId { get; set; }
    
    public Guid ObjectId { get; set; }
    
    public long UserId { get; set; }
    
    public int ParticleCount { get; set; }
    
    public double GlobalSize { get; set; }
    
    public double GenerationZoneVolume { get; set; }
    
    public int GenerationZoneForm { get; set; }

    public double NumericalConcentration { get; set; }
    
    public float MaxParticleSize { get; set; }
    
    public float MinParticleSize { get; set; }
    
    public double Excess { get; set; }
    
    public float K { get; set; }
    
    public float Theta { get; set; }
    
    public DateTime GenerationStart { get; set; }
    
    public DateTime GenerationEnd { get; set; }
    
    public DateTime InputDate { get; set; }
}