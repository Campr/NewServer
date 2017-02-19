namespace Server.Lib.Configuration
{
    public interface IServerConfiguration
    {
        IAwsConfiguration Aws { get; }
        IAzureConfiguration Azure { get; }
    }
}