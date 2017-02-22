using System;
using Server.Lib.Models.Resources.Api;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Models.Resources
{
    public class User : VersionedResource
    {
        public User(CacheUser cacheUser)
        {
            
        }

        public string Handle { get; set; }
        public string Entity { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool? IsBotFollowed { get; set; }
        public DateTime? LastDiscoveryAttempt { get; set; }

        public override string[] CacheIds => new [] { this.Id, this.Email, this.Entity };

        public override ApiResource ToApi()
        {
            throw new NotImplementedException();
        }

        public override CacheResource ToDb()
        {
            return new CacheUser
            {
                Id = this.Id,
                CreatedAt = this.CreatedAt,
                DeletedAt = this.DeletedAt,
                VersionId = this.VersionId,
                VersionCreatedAt = this.VersionCreatedAt,
                Handle = this.Handle,
                Entity = this.Entity,
                Email = this.Email,
                Password = this.Password,
                PasswordSalt = this.PasswordSalt,
                IsBotFollowed = this.IsBotFollowed,
                LastDiscoveryAttempt = this.LastDiscoveryAttempt
            };
        }
    }
}