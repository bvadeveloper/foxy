using System.Collections.Generic;

namespace Platform.Logging
{
    public class LogProperty
    {
        public string Key { get; }

        public object Value { get; }

        public LogProperty(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public LogProperty(string key, IReadOnlyDictionary<string, string> value)
        {
            Key = key;
            Value = value;
        }
    }
}
