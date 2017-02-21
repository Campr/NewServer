using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Server.Lib.Infrastructure;

namespace Server.Lib.Connectors.Blobs.Aws
{
    public class AwsBlobs : Connector, IBlobs
    {
        public AwsBlobs(IConfiguration configuration)
        {
            Ensure.Argument.IsNotNull(configuration, nameof(configuration));

            // Create the credentials for our AWS account.
            var awsCredentials = new BasicAWSCredentials(
                configuration.AwsAccessKey,
                configuration.AwsAccessSecret);

            // Create the underlying S3 client.
            var region = RegionEndpoint.GetBySystemName(configuration.AwsBlobsRegion);
            var client = new AmazonS3Client(awsCredentials, region);

            // Create our blob containers.
            this.PostVersions = new AwsBlobContainer(client, "postversions", string.Empty);
            this.Attachments = new AwsBlobContainer(client, "attachments", string.Empty);
        }

        public IBlobContainer PostVersions { get; }
        public IBlobContainer Attachments { get; }
    }
}