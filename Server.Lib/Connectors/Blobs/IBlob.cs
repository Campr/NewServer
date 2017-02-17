using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Lib.Connectors.Blobs
{
    public interface IBlob
    {
        Task<Stream> DownloadStreamAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task UploadFromStreamAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken));
    }
}