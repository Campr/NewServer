using System;

namespace Server.Lib.Helpers
{
    public interface IUriHelpers
    {
        bool TryGetHandle(Uri internalEntity, out string handle);
    }
}