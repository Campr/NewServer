using System.Collections.Generic;

namespace Server.Lib.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            TValue result;
            return dict.TryGetValue(key, out result) 
                ? result 
                : defaultValue;
        }
    }
}