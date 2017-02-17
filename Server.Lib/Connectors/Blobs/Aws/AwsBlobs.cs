using Amazon;
using Amazon.Runtime;
using Amazon.S3;

namespace Server.Lib.Connectors.Blobs.Aws
{
    public class AwsBlobs : Connector, IBlobs
    {
        public AwsBlobs()
        {
            // Create the credentials for our AWS account.
            var awsCredentials = new BasicAWSCredentials("", "");

            // Create the underlying S3 client.
            var client = new AmazonS3Client(awsCredentials, RegionEndpoint.SAEast1);

            // Create our blob containers.
            this.PostVersions = new AwsBlobContainer(client, "postversions", "");
            this.Attachments = new AwsBlobContainer(client, "attachments", "");
        }

        public IBlobContainer PostVersions { get; }
        public IBlobContainer Attachments { get; }
    }
}