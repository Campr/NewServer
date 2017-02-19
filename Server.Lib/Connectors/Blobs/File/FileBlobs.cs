using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Configuration;
using Server.Lib.Infrastructure;

namespace Server.Lib.Connectors.Blobs.File
{
    public class FileBlobs : Connector, IBlobs
    {
        public FileBlobs(ILocalConfiguration localConfiguration)
        {
            Ensure.Argument.IsNotNull(localConfiguration, nameof(localConfiguration));

            // Create the paths of the local containers.
            this.postVersionsPath = Path.Combine(localConfiguration.BlobFilePath, "postversions");
            this.attachmentsPath = Path.Combine(localConfiguration.BlobFilePath, "attachments");

            // Create the IBlobContainer objects.
            this.PostVersions = new FileBlobContainer(this.postVersionsPath);
            this.Attachments = new FileBlobContainer(this.attachmentsPath);
        }
        
        private readonly string postVersionsPath;
        private readonly string attachmentsPath;

        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            // Make sure the folders for the local containers exist.
            Directory.CreateDirectory(this.postVersionsPath);
            Directory.CreateDirectory(this.attachmentsPath);

            return base.InitializeAsync(cancellationToken);
        }

        public IBlobContainer PostVersions { get; }
        public IBlobContainer Attachments { get; }
    }
}