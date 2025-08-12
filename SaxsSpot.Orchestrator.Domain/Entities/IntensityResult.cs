namespace SaxsSpot.Orchestrator.Domain.Entities;

/// <summary>
/// Entity for object storage to storage a result of calculating
/// </summary>
public class IntensityResult
{
    public IntensityResult(float qVector, float intensity)
    {
        QVector = qVector;
        Intensity = intensity;
    }
    
    public float QVector { get; set; }

    public float Intensity { get; set; }

    public override string ToString()
    {
        return $"{QVector} {Intensity}";
    }
}