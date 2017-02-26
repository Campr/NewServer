using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;

namespace Server.Lib.Connectors.Tables.Mongo
{
    public class SnakeCaseElementNameConvention : ConventionBase, IMemberMapConvention
    {
        public SnakeCaseElementNameConvention(ITextHelpers textHelpers)
        {
            Ensure.Argument.IsNotNull(textHelpers, nameof(textHelpers));
            this.textHelpers = textHelpers;
        }

        private readonly ITextHelpers textHelpers;
        
        public void Apply(BsonMemberMap memberMap)
        {
            var newName = this.textHelpers.ToJsonPropertyName(memberMap.MemberName);
            memberMap.SetElementName(newName);
        }
    }
}