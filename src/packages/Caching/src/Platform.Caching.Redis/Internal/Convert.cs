using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Platform.Caching.Redis.Internal
{
    internal static class Convert
    {
        private static readonly JsonSerializerSettings Settings = new()
        {
            ContractResolver = new OrderedContractResolver()
        };

        public static string Serialize(object value) =>
            JsonConvert.SerializeObject(value, Formatting.None, Settings);

        public static T Parse<T>(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception)
            {
                // Can't deserialize into the {typeof(T).Name} type.
            }

            return default;
        }

        private class OrderedContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty>
                CreateProperties(Type type, MemberSerialization memberSerialization) =>
                base
                    .CreateProperties(type, memberSerialization)
                    .OrderBy(p => p.PropertyName)
                    .ToList();
        }
    }
}