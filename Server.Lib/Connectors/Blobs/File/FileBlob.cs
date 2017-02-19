using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Infrastructure;

namespace Server.Lib.Connectors.Blobs.File
{
    public class FileBlob : IBlob
    {
        public FileBlob(string path)
        {
            Ensure.Argument.IsNotNullOrWhiteSpace(path, nameof(path));
            this.path = path;
        }

        private readonly string path;

        public Task<Stream> DownloadStreamAsync(CancellationToken cancellationToken)
        {
            // Open the local file representing our blob for reading.
            return Task.FromResult(new FileStream(this.path, FileMode.Open, FileAccess.Read) as Stream);
        }

        public async Task UploadFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            // Open the local file repesenting our blob for writing.
            using (var blobStream = new FileStream(this.path, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(blobStream, 4096, cancellationToken);
            }
        }
    }
}