using System;

namespace Server.Lib.Models.Resources
{
    public class Resource
    {
        public string Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}