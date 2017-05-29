using System;

namespace Server.Lib.Helpers
{
    public interface IUriHelpers
    {
        bool TryGetHandleFromPath(string requestPath, out string handle);
        bool TryGetHandleFromEntity(Uri internalEntity, out string handle);
        Uri ApiUriFromUri(Uri uri);
    }
}