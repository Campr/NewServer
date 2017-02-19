using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Server.Lib.Infrastructure;

namespace Server.Lib.Connectors.Blobs.Aws
{
    public class AwsBlob : IBlob
    {
        public AwsBlob(
            AmazonS3Client client, 
            string bucketName, 
            string key)
        {
            Ensure.Argument.IsNotNull(client, nameof(client));
            Ensure.Argument.IsNotNullOrWhiteSpace(bucketName, nameof(bucketName));
            Ensure.Argument.IsNotNullOrWhiteSpace(key, nameof(key));

            this.client = client;
            this.bucketName = bucketName;
            this.key = key;
        }

        private readonly AmazonS3Client client;
        private readonly string bucketName;
        private readonly string key;

        public async Task<Stream> DownloadStreamAsync(CancellationToken cancellationToken)
        {
            // Create the S3 request we'll be using to download our data.
            var getRequest = new GetObjectRequest
            {
                BucketName = this.bucketName,
                Key = this.key
            };

            // Perform the request.
            var response = await this.client.GetObjectAsync(getRequest, cancellationToken);
            return response.ResponseStream;
        }

        public Task UploadFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            // Create the S3 request we'll be using to upload our data.
            var putRequest = new PutObjectRequest
            {
                BucketName = this.bucketName,
                Key = this.key,
                InputStream = stream
            };

            // Perform the request.
            return this.client.PutObjectAsync(putRequest, cancellationToken);
        }
    }
}