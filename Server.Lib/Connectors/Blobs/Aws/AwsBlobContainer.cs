using Amazon.S3;
using Server.Lib.Extensions;

namespace Server.Lib.Connectors.Blobs.Aws
{
    public class AwsBlobContainer : IBlobContainer
    {
        public AwsBlobContainer(
            AmazonS3Client client, 
            string bucketName, 
            string basePath)
        {
            Ensure.Argument.IsNotNull(client, nameof(client));
            Ensure.Argument.IsNotNullOrWhiteSpace(bucketName, nameof(bucketName));
            Ensure.Argument.IsNotNullOrWhiteSpace(basePath, nameof(basePath));

            this.client = client;
            this.bucketName = bucketName;
            this.basePath = basePath;
        }

        private readonly AmazonS3Client client;
        private readonly string bucketName;
        private readonly string basePath;

        public IBlob GetBlob(string name)
        {
            Ensure.Argument.IsNotNullOrWhiteSpace(name, nameof(name));
            return new AwsBlob(this.client, this.bucketName, this.basePath + name);
        }

    }
}