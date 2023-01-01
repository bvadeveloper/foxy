using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog.Events;
using Serilog.Formatting;

namespace Platform.Logging.Host
{
    internal class JsonTextFormatter : ITextFormatter
    {
        private static readonly Regex ValidPropertyName = new Regex(@"^[a-zA-Z0-9-_]+$");

        static readonly Dictionary<string, string> DefaultContext =
            new Dictionary<string, string>
            {
                ["id"] = "00000000-0000-0000-0000-000000000000",
                ["origin"] = Assembly.GetEntryAssembly()?.GetName().Name ?? "<unresolved>",
            };

        private static readonly Dictionary<string, MsgRender> Renderers =
            new Dictionary<string, MsgRender>
            {
                { "message", MessageRenderer },
                { "class", ClassRenderer },
            };

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
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
            Converters = new JsonConverter[] { new Newtonsoft.Json.Converters.StringEnumConverter() },
        };

        private readonly LogProperty[] _rootProperties;
        private readonly string _format;
        private readonly HashSet<string> _suppressProperties;

        public JsonTextFormatter(LoggingSettings settings, IEnumerable<LogProperty> rootProperties)
        {
            Settings.Formatting = settings.Formatting;
            _suppressProperties = settings.SuppressProperties();
            _rootProperties = rootProperties.ToArray();
            _format = string.IsNullOrWhiteSpace(settings.MessageFormat) ? string.Empty : settings.MessageFormat;
        }

        private delegate string MsgRender(
            MessageTemplate template,
            string prop,
            IReadOnlyDictionary<string, object?> props1,
            IReadOnlyDictionary<string, object?> props2);

        public void Format(LogEvent logEvent, TextWriter output)
        {
            var entry = new Dictionary<string, object?>();

            // "context" is what we always want to have in the root scope
            entry["context"] = DefaultContext;

            foreach (var property in _rootProperties)
            {
                entry[property.Key] = property.Value;
            }

            foreach (var pair in logEvent.Properties)
            {
                if (!pair.Key.StartsWith(Constants.LogProperties.RootPropertyPrefix))
                {
                    continue;
                }

                var key = pair.Key.Substring(Constants.LogProperties.RootPropertyPrefix.Length);
                entry[key] = PropertyValue(pair.Value);
            }

            var props = Translate(_suppressProperties, logEvent.Properties);
            entry["timestamp"] = logEvent.Timestamp.ToUniversalTime();
            entry["level"] = RenderLevel(logEvent.Level.ToString());
            entry["exception"] = logEvent.Exception != null ? new ExceptionInfo(logEvent.Exception) : ExceptionInfo.Empty;
            entry["props"] = props;
            entry["message"] = RenderMessage(_format, logEvent.MessageTemplate, props, entry);

            output.WriteLine(JsonConvert.SerializeObject(entry, Settings));
        }

        private static string RenderLevel(string level) =>
            level.Equals("Verbose") ? "Trace" : level;

        private static string MessageRenderer(
            MessageTemplate template,
            string prop,
            IReadOnlyDictionary<string, object?> props1,
            IReadOnlyDictionary<string, object?> props2) =>
            template.Text;

        private static string ClassRenderer(
            MessageTemplate template,
            string prop,
            IReadOnlyDictionary<string, object?> props1,
            IReadOnlyDictionary<string, object?> props2) =>
            props1.ContainsKey("SourceContext")
                ? props1["SourceContext"]?.ToString()?.Split('.').LastOrDefault() ?? string.Empty
                : string.Empty;

        private static string PropertyFormatter(
            MessageTemplate template,
            string prop,
            IReadOnlyDictionary<string, object?> props1,
            IReadOnlyDictionary<string, object?> props2)
        {
            if (props1.ContainsKey(prop))
            {
                return props1[prop]?.ToString() ?? string.Empty;
            }

            if (props2.ContainsKey(prop))
            {
                return props2[prop]?.ToString() ?? string.Empty;
            }

            return string.Empty;
        }

        private static string RenderMessage(
            string? messageFormat,
            MessageTemplate template,
            IReadOnlyDictionary<string, object?> props1,
            IReadOnlyDictionary<string, object?> props2)
        {
            if (string.IsNullOrEmpty(messageFormat) || string.IsNullOrWhiteSpace(messageFormat))
            {
                return template.Text;
            }

            var r = new Regex(@"\{\{(\w*)\}\}");
            var result = r.Replace(
                messageFormat,
                x =>
                {
                    var prop = x.Groups[1].Value;
                    var render = Renderers.GetValueOrDefault(prop, PropertyFormatter) ?? PropertyFormatter;
                    return render(template, prop, props1, props2).Trim();
                });

            return result.Trim();
        }

        private static object PropertyValue(LogEventPropertyValue? val)
        {
            const string nullLiteral = "";

            switch (val)
            {
                case ScalarValue scalarValue:
                    var obj = scalarValue.Value;
                    switch (obj)
                    {
                        case null:
                            return nullLiteral;
                        case byte _:
                        case short _:
                        case ushort _:
                        case int _:
                        case uint _:
                        case long _:
                        case ulong _:
                        case float _:
                        case double _:
                            return scalarValue.Value;
                        case TimeSpan ts:
                            return ts.TotalMilliseconds;
                        case DateTime _:
                        case DateTimeOffset _:
                        {
                            var quoted = JsonConvert.SerializeObject(obj, Settings);
                            var result = quoted.Substring(1, quoted.Length - 2);
                            return result;
                        }

                        default:
                            return scalarValue.Value?.ToString() ?? nullLiteral;
                    }

                case DictionaryValue dictionaryValue:
                {
                    var d = new Dictionary<string, object>();
                    foreach (var (k, v) in dictionaryValue.Elements)
                    {
                        var key = k?.Value?.ToString();
                        if (key != null)
                        {
                            d[key] = PropertyValue(v);
                        }
                    }

                    return d;
                }

                default:
                    // Q: Do we want to have [SequenceValue, StructureValue] support here?
                    return val?.GetType().ToString() ?? nullLiteral;
            }
        }

        private static IReadOnlyDictionary<string, object?> Translate(HashSet<string> suppressProperties, IReadOnlyDictionary<string, LogEventPropertyValue> properties)
        {
            var invalidProperties = new List<string>();
            var ps = new Dictionary<string, object?>();
            foreach (var (k, v) in properties)
            {
                if (!ValidPropertyName.IsMatch(k))
                {
                    if (k.StartsWith(Constants.LogProperties.RootPropertyPrefix))
                    {
                        invalidProperties.Add(k.Substring(Constants.LogProperties.RootPropertyPrefix.Length));
                    }
                    else
                    {
                        invalidProperties.Add(k);
                    }

                    continue;
                }

                if (k.StartsWith(Constants.LogProperties.RootPropertyPrefix))
                {
                    continue;
                }

                if (suppressProperties.Contains(k))
                {
                    continue;
                }

                ps[k] = PropertyValue(v);
            }

            if (invalidProperties.Count > 0)
            {
                ps["_invalidProperties"] = invalidProperties;
            }

            return ps;
        }
    }
}
