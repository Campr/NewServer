namespace Server.Lib.Configuration
{
    public interface IAwsConfiguration
    {
        string AwsAccessKey { get; }
        string AwsAccessSecret { get; }
        string BlobsRegion { get; }
    }
}