using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Server.Lib.Infrastructure;

namespace Server.Lib.Connectors.Blobs.Azure
{
    public class AzureBlobs : Connector, IBlobs
    {
        public AzureBlobs(IConfiguration configuration)
        {
            Ensure.Argument.IsNotNull(configuration, nameof(configuration));

            // Create the storage account from the connection stirng, and the corresponding client.
            var blobsStorageAccount = CloudStorageAccount.Parse(configuration.AzureBlobsConnectionString);
            var blobsClient = blobsStorageAccount.CreateCloudBlobClient();

            // Create the blob container references.
            this.postVersionsContainer = blobsClient.GetContainerReference("postversions");
            this.attachmentsContainer = blobsClient.GetContainerReference("attachments");

            // Create IBlobContainer objects.
            this.PostVersions = new AzureBlobContainer(this.postVersionsContainer);
            this.Attachments = new AzureBlobContainer(this.attachmentsContainer);

            // Create the initializer for this component.
            this.initializer = new TaskRunner(this.InitializeOnceAsync);
        }

        private readonly TaskRunner initializer;
        private readonly CloudBlobContainer postVersionsContainer;
        private readonly CloudBlobContainer attachmentsContainer;

        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            return this.initializer.RunOnce(cancellationToken);
        }

        private Task InitializeOnceAsync(CancellationToken cancellationToken)
        {
            // Try to create the containers.
            return Task.WhenAll(
                this.postVersionsContainer.CreateIfNotExistsAsync(
                    BlobContainerPublicAccessType.Off,
                    null, null, cancellationToken),
                this.attachmentsContainer.CreateIfNotExistsAsync(
                    BlobContainerPublicAccessType.Off,
                    null, null, cancellationToken)
            );
        }

        public IBlobContainer PostVersions { get; }
        public IBlobContainer Attachments { get; }
    }
}