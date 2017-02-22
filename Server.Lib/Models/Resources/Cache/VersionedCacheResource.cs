using System;

namespace Server.Lib.Models.Resources.Cache
{
    public class CacheVersionedResource : CacheResource
    {
        public string VersionId { get; set; }
        public DateTime VersionCreatedAt { get; set; }

        public int CompareTo(CacheVersionedResource otherResource)
        {
            // Make sure we were given something to compare.
            if (otherResource == null)
                return -1;

            // If the dates are equal, compare by Id.
            if (this.VersionCreatedAt == otherResource.VersionCreatedAt)
                return string.Compare(this.VersionId, otherResource.VersionId, StringComparison.OrdinalIgnoreCase);

            // Otherwise, compare by date.
            return this.VersionCreatedAt.CompareTo(otherResource.VersionCreatedAt);
        }
    }
}