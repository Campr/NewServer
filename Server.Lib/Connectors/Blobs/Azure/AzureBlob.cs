using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Server.Lib.Extensions;

namespace Server.Lib.Connectors.Blobs.Azure
{
    public class AzureBlob : IBlob
    {
        public AzureBlob(CloudBlockBlob baseBlockBlob)
        {
            Ensure.Argument.IsNotNull(baseBlockBlob, nameof(baseBlockBlob));
            this.baseBlockBlob = baseBlockBlob;
        }

        private readonly CloudBlockBlob baseBlockBlob;

        public Task<Stream> DownloadStreamAsync(CancellationToken cancellationToken)
        {
            return this.baseBlockBlob.OpenReadAsync(
                AccessCondition.GenerateIfExistsCondition(),
                null, null, cancellationToken);
        }

        public Task UploadFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            return this.baseBlockBlob.UploadFromStreamAsync(
                stream,
                AccessCondition.GenerateIfExistsCondition(),
                null, null, cancellationToken);
        }
    }
}