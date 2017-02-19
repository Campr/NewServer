namespace Server.Lib.Configuration
{
    public interface IAzureConfiguration
    {
        string BlobsConnectionString { get; }
    }
}