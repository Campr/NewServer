using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Server.Lib.Configuration;
using Server.Lib.Infrastructure;

namespace Server.Lib.Connectors.Blobs.Aws
{
    public class AwsBlobs : Connector, IBlobs
    {
        public AwsBlobs(IAwsConfiguration awsConfiguration)
        {
            Ensure.Argument.IsNotNull(awsConfiguration, nameof(awsConfiguration));

            // Create the credentials for our AWS account.
            var awsCredentials = new BasicAWSCredentials(
                awsConfiguration.AwsAccessKey,
                awsConfiguration.AwsAccessSecret);

            // Create the underlying S3 client.
            var region = RegionEndpoint.GetBySystemName(awsConfiguration.BlobsRegion);
            var client = new AmazonS3Client(awsCredentials, region);

            // Create our blob containers.
            this.PostVersions = new AwsBlobContainer(client, "postversions", string.Empty);
            this.Attachments = new AwsBlobContainer(client, "attachments", string.Empty);
        }

        public IBlobContainer PostVersions { get; }
        public IBlobContainer Attachments { get; }
    }
}