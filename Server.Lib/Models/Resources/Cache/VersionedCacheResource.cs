using System;
using Server.Lib.Extensions;

namespace Server.Lib.Models.Resources.Cache
{
    public class VersionedCacheResource : CacheResource
    {
        public override string DbId => $"{this.Id}-{this.VersionId}";
        public string VersionId { get; set; }
        public DateTime OriginalCreatedAt { get; set; }

        public int CompareTo(VersionedCacheResource otherResource)
        {
            // Make sure we were given something to compare.
            if (otherResource == null)
                return -1;

            // If the dates are equal, compare by Id.
            var dateCompare = this.CreatedAt.CompareToMillisecond(otherResource.CreatedAt);
            if (dateCompare == 0)
                return string.Compare(this.VersionId, otherResource.VersionId, StringComparison.OrdinalIgnoreCase);

            // Otherwise, compare by date.
            return dateCompare;
        }
    }
}