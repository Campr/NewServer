using System.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Server.Lib.Connectors.Tables.Mongo
{
    public class SnakeCaseElementNameConvention : ConventionBase, IMemberMapConvention
    {
        public void Apply(BsonMemberMap memberMap)
        {
            var newName = this.GetElementName(memberMap.MemberName);
            memberMap.SetElementName(newName);
        }

        private string GetElementName(string memberName)
        {
            // Make sure we have something to work with.
            if (string.IsNullOrWhiteSpace(memberName))
                return memberName;

            // Insert an underscore before all uppercase chars.
            memberName = string.Concat(memberName.Select((c, i) =>
                i > 0 && char.IsUpper(c) ? "_" + c.ToString() : c.ToString()));

            // Convert to lowercase and return.
            return memberName.ToLowerInvariant();
        }
    }
}