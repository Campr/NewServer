using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Server.Lib.Configuration;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;
using Server.Lib.Services;

namespace Server.Lib.Connectors.Blobs.Azure
{
    public class AzureBlobs : Connector, IBlobs
    {
        public AzureBlobs(
            ILoggingService loggingService,
            ITaskHelpers taskHelpers,
            IAzureConfiguration configuration)
        {
            Ensure.Argument.IsNotNull(loggingService, nameof(loggingService));
            Ensure.Argument.IsNotNull(taskHelpers, nameof(taskHelpers));
            Ensure.Argument.IsNotNull(configuration, nameof(configuration));

            this.loggingService = loggingService;
            this.taskHelpers = taskHelpers;

            // Create the storage account from the connection stirng, and the corresponding client.
            var blobsStorageAccount = CloudStorageAccount.Parse(configuration.BlobsConnectionString);
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

        private readonly ILoggingService loggingService;
        private readonly ITaskHelpers taskHelpers;

        private readonly TaskRunner initializer;
        private readonly CloudBlobContainer postVersionsContainer;
        private readonly CloudBlobContainer attachmentsContainer;

        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            return this.initializer.RunOnce(cancellationToken);
        }

        private async Task InitializeOnceAsync(CancellationToken cancellationToken)
        {
            // Try to create the containers.
            try
            {
                await this.taskHelpers.RetryAsync(() => 
                    Task.WhenAll(
                        this.postVersionsContainer.CreateIfNotExistsAsync(
                            BlobContainerPublicAccessType.Off,
                            null, null, cancellationToken),
                        this.attachmentsContainer.CreateIfNotExistsAsync(
                            BlobContainerPublicAccessType.Off,
                            null, null, cancellationToken)
                    ),
                    cancellationToken);
            }
            catch (Exception ex)
            {
                this.loggingService.Exception(ex, "Error during Azure blob initialization. We won't retry.");
                throw;
            }
        }

        public IBlobContainer PostVersions { get; }
        public IBlobContainer Attachments { get; }
    }
}