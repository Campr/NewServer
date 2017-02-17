namespace Server.Lib.Connectors.Blobs
{
    public interface IBlobs : IConnector
    {
        IBlobContainer PostVersions { get; }
        IBlobContainer Attachments { get; }
    }
}