namespace Server.Lib.Models.Resources.Cache.Posts
{
    public class CachePostReference
    {
        // User.
        public string UserId { get; set; }
        public string OriginalEntity { get; set; }

        // Post.
        public string PostId { get; set; }
        public string VersionId { get; set; }
        public string Type { get; set; }

        // Reference.
        public string Role { get; set; }
        public bool IsPublic { get; set; }
    }
}