using System.IO;
using Server.Lib.Infrastructure;

namespace Server.Lib.Connectors.Blobs.File
{
    public class FileBlobContainer : IBlobContainer
    {
        public FileBlobContainer(string path)
        {
            Ensure.Argument.IsNotNullOrWhiteSpace(path, nameof(path));
            this.path = path;
        }

        private readonly string path;

        public IBlob GetBlob(string name)
        {
            Ensure.Argument.IsNotNullOrWhiteSpace(name, nameof(name));
            return new FileBlob(Path.Combine(this.path, name));
        }
    }
}