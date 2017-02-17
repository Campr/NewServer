using Microsoft.WindowsAzure.Storage.Blob;
using Server.Lib.Extensions;

namespace Server.Lib.Connectors.Blobs.Azure
{
    public class AzureBlobContainer : IBlobContainer
    {
        public AzureBlobContainer(CloudBlobContainer baseContainer)
        {
            Ensure.Argument.IsNotNull(baseContainer, nameof(baseContainer));
            this.baseContainer = baseContainer;
        }

        private readonly CloudBlobContainer baseContainer;

        public IBlob GetBlob(string name)
        {
            Ensure.Argument.IsNotNullOrWhiteSpace(name, nameof(name));
            var blockBlobReference = this.baseContainer.GetBlockBlobReference(name);
            return new AzureBlob(blockBlobReference);
        }
    }
}