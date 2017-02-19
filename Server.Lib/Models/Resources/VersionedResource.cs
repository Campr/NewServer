using System;

namespace Server.Lib.Models.Resources
{
    public class VersionedResource : Resource
    {
        public string VersionId { get; set; }
        public DateTime? VersionCreatedAt { get; set; }
    }
}