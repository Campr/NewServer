using System;
using Server.Lib.Extensions;

namespace Server.Lib.Models.Resources
{
    public abstract class VersionedResource : Resource
    {
        public string VersionId { get; set; }
        public DateTime OriginalCreatedAt { get; set; }

        public int CompareTo(VersionedResource otherResource)
        {
            // Make sure we were given something to compare.
            if (otherResource == null)
                return -1;

            // If the dates are equal, compare by Id.
            var dateCompare = this.CreatedAt.CompareToMillisecond(otherResource.CreatedAt);
            if (this.CreatedAt == otherResource.CreatedAt)
                return string.Compare(this.VersionId, otherResource.VersionId, StringComparison.OrdinalIgnoreCase);

            // Otherwise, compare by date.
            return dateCompare;
        }
    }
}