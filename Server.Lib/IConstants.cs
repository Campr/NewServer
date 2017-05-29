using System;
using Server.Lib.Models.Resources.Posts;

namespace Server.Lib
{
    public interface IConstants
    {
        string PostContentType { get; }

        PostType MetaPostType { get; }
        PostType AppPostType { get; }
        PostType AppAuthorizationPostType { get; }
        PostType RelationshipPostType { get; }
        PostType SubscriptionPostType { get; }
        PostType CredentialsPostType { get; }
        PostType DeletePostType { get; }
        PostType DeliveryFailurePostType { get; }
        PostType CamprProfilePostType { get; }

        string MetaPostRel { get; }

        TimeSpan HawkTimestampThreshold { get; }
    }
}