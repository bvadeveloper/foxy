using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Platform.Logging.Extensions
{
    public static partial class LoggerExtensions
    {
        public static IDisposable WithRoot(this ILogger logger, string propertyName, object? propertyValue)
        {
            var state = new Dictionary<string, object?>
            {
                { NormalizeRootName(propertyName), NormalizeValue(propertyValue) }
            };
            return logger.BeginScope(state);
        }

        public static IDisposable With(this ILogger logger, string propertyName, object? propertyValue)
            => logger.With((propertyName, propertyValue));

        public static IDisposable With(this ILogger logger, params (string PropertyName, object? PropertyValue)[] props)
        {
            var state = props
                .ToDictionary(
                    x => NormalizeName(x.PropertyName),
                    x => NormalizeValue(x.PropertyValue));
            return logger.BeginScope(state);
        }

        private static string NormalizeName(string propertyName) => $"{propertyName}";

        private static string NormalizeRootName(string propertyName) => $"____root__{propertyName}";

        private static readonly HashSet<Type> BasicTypes = new()
        {
            typeof(string),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(TimeSpan),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(Guid),
        };
        
        private static object NormalizeValue(object? obj)
        {
            static object Value(object? val)
            {
                switch (val)
                {
                    case null:
                        return string.Empty;
                    case string _:
                        return val;
                    case byte _:
                    case short _:
                    case ushort _:
                    case int _:
                    case uint _:
                    case long _:
                    case ulong _:
                    case float _:
                    case double _:
                        return val;
                    case TimeSpan ts:
                        return ts.TotalMilliseconds;
                    case decimal dec:
                        return dec.ToString(CultureInfo.InvariantCulture);
                    case DateTime _:
                    case DateTimeOffset _:
                    {
                        var quoted = JsonConvert.SerializeObject(val, Settings);
                        var result = quoted.Substring(1, quoted.Length - 2);
                        return result;
                    }
                    case Guid guid:
                        return guid.ToString("D", CultureInfo.InvariantCulture);
                }
                
                try
                {
                    return JsonConvert.SerializeObject(val, Settings);
                }
                catch (Exception)
                {
                    return "<SerializationFailure>";
                }
            }

            switch (obj)
            {
                case null:
                    return string.Empty;
                case IDictionary<string, string> dictOfStrToStr:
                    var dictType = dictOfStrToStr.GetType();
                    // serilog translates dictionary to the SequenceValue string
                    return (dictType.FullName ?? string.Empty).StartsWith("System.Collections.Immutable.ImmutableDictionary") 
                        ? new Dictionary<string, string>(dictOfStrToStr)  
                        : obj;
                case IDictionary<string, object?> d:
                    return d
                        .ToDictionary(
                            x => x.Key,
                            x => Value(x.Value));
            }

            if (BasicTypes.Contains(obj.GetType()))
            {
                return Value(obj);
            }
            
            var dict = new Dictionary<string, object?>();
            foreach (var pi in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                try
                {
                    var v = pi.GetValue(obj, null);
                    dict[pi.Name] = Value(v);
                }
                catch (Exception)
                {
                    dict[pi.Name] = "<SerializationFailure>";
                }
            }
            return dict;
        }

        private static readonly JsonSerializerSettings Settings = new()
        {
            MaxDepth = 8,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateFormatString = "yyyy-MM-ddTHH:mm:ss.ffffffZ",
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new JsonConverter[] {new Newtonsoft.Json.Converters.StringEnumConverter()}
        };
    }
}
