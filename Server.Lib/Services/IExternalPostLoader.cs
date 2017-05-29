using System.Threading.Tasks;
using Server.Lib.Models.Resources;

namespace Server.Lib.Services
{
    public interface IExternalPostLoader
    {
        // Post storage strategy:
        // #1. Stored directly in file storage (replicated).
        // #2. We keep one reference with User -> Post -> Version in DB for looking for corresponding file (& additional metadata like region, etc.).
        // #3. Post metadata is stored at the user level (even when not the author). In DB.
        //
        // Loading a post => find at the user level, find the DB entry for path, download from cache or storage.
        // Result: less load on DB. Use of simple files easier to scale eventually.
    }
}