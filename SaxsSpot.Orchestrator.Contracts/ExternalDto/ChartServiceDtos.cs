namespace SaxsSpot.Orchestrator.Contracts.ExternalDto;

public class PlotRequest
{
    public string title { get; set; }
    public string x_label { get; set; }
    public string y_label { get; set; }
    public bool x_log_scale { get; set; }
    public bool y_log_scale { get; set; }
    public Dataset[] datasets { get; set; }
}


public class Dataset
{
    public string id { get; set; }
    public double[] x { get; set; }
    public double[] y { get; set; }
	
    public void SortByX()
    {
        if (x.Length != y.Length)
        {
            throw new InvalidOperationException("Length of x and y arrays must be equal.");
        }

        Array.Sort(x, y);
    }
}