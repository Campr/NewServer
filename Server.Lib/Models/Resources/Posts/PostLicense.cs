using Server.Lib.Infrastructure;

namespace Server.Lib.Models.Resources.Posts
{
    public class PostLicense
    {
        public PostLicense(
            string id,
            string name,
            string url)
        {
            Ensure.Argument.IsNotNullOrWhiteSpace(id, nameof(id));
            Ensure.Argument.IsNotNullOrWhiteSpace(name, nameof(name));
            Ensure.Argument.IsNotNullOrWhiteSpace(url, nameof(url));

            this.Id = id;
            this.Name = name;
            this.Url = url;
        }

        public string Id { get; }
        public string Name { get; }
        public string Url { get; }
    }
}