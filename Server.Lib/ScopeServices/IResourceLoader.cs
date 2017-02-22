using System.Threading.Tasks;
using Server.Lib.Models.Resources;

namespace Server.Lib.ScopeServices
{
    public interface IResourceLoader
    {
        Task<TResource> LoadFromString<TResource>(string json) where TResource : Resource;
    }
}