using Newtonsoft.Json.Serialization;
using Server.Lib.Infrastructure;

namespace Server.Lib.Helpers
{
    class BaseContractResolver : DefaultContractResolver
    {
        public BaseContractResolver(ITextHelpers textHelpers)
        {
            Ensure.Argument.IsNotNull(textHelpers, nameof(textHelpers));
            this.textHelpers = textHelpers;
        }

        private readonly ITextHelpers textHelpers;

        protected override string ResolvePropertyName(string propertyName)
        {
            // Rewrite the property name to use camel case and remove the Id.
            return this.textHelpers.ToJsonPropertyName(propertyName);
        }
    }
}