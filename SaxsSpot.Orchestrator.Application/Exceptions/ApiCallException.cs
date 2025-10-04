namespace SaxsSpot.Orchestrator.Application.Exceptions;

public class ApiCallException(string serviceName, Exception innerException) 
    : Exception(innerException.Message, innerException)
{
    public readonly string ServiceName = serviceName;
}