namespace Server.Lib.Connectors.Blobs.Azure
{
    public class AzureBlobs : Connector, IBlobs
    {
        public AzureBlobs()
        {
            
        }

        public IBlobContainer PostVersions { get; }
        public IBlobContainer Attachments { get; }
    }
}