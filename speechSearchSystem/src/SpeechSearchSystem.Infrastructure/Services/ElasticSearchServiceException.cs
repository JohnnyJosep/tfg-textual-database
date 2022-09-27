namespace SpeechSearchSystem.Infrastructure.Services;

public class ElasticSearchServiceException : Exception
{
    public ElasticSearchServiceException(string error, Exception innerException) : base(error, innerException)
    {
    }
}