using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Platform.Logging.Host
{
    internal class LoggingSettings
    {
        private readonly HashSet<string> _suppressProperties = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            "connectionId",
            "parentId",
            "requestId",
            "spanId",
            "traceId",
        };

        private IDictionary<string, LogLevel> _logLevel = new Dictionary<string, LogLevel>();

        private string? _suppress;

        public Formatting Formatting { get; set; } = Formatting.None;

        public bool RecordApiCall { get; set; } = true;

        public string MessageFormat { get; set; } = string.Empty;
        
        public string Suppress
        {
            get => _suppress ?? string.Empty;
            
            set
            {
                var suppress = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
                _suppress = suppress;
                _suppressProperties.Clear();
                foreach (var item in suppress.Split(','))
                {
                    _suppressProperties.Add(item.Trim().ToLowerInvariant());
                }
            }
        }

        /// <summary>
        /// Filters for logs in this case did not work properly, so need change 'Key' to correct format.
        /// </summary>
        /// <remarks>
        /// This LogLevel does not affect settings such as `Microsoft.AspNetCore`
        /// Because it translates `Microsoft.AspNetCore` -> `Microsoft.aspnetcore`, which the logging facility ignores.
        /// </remarks>
        public IDictionary<string, LogLevel> LogLevel
        {
            get => _logLevel;
            
            set
            {
                var next = new Dictionary<string, LogLevel>();

                var normalized = value
                    .GroupBy(e => e.Key.ToUpper())
                    .ToDictionary(
                        g => char.ToUpper(g.Key[0]) + g.Key.ToLower()[1..],
                        g => g.First().Value);

                foreach (var p in value)
                {
                    next[p.Key] = p.Value;
                }

                foreach (var p in normalized)
                {
                    next[p.Key] = p.Value;
                }

                _logLevel = next;
            }
        }

        public HashSet<string> SuppressProperties() => _suppressProperties;
    }
}
