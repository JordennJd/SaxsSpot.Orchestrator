namespace SaxsSpot.Orchestrator.Domain.Entities;

/// <summary>
/// Entity for object storage to storage a result of calculating
/// </summary>
public class IntensityResult
{
    public IntensityResult(double qVector, double intensity)
    {
        QVector = qVector;
        Intensity = intensity;
    }
    
    public double QVector { get; set; }

    public double Intensity { get; set; }

    public override string ToString()
    {
        return $"{QVector} {Intensity}";
    }
}