namespace Server.Lib.Connectors.Blobs
{
    public interface IBlobContainer
    {
        IBlob GetBlob(string name);
    }
}