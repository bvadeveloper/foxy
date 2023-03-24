using Platform.Geolocation.HostLocation.Stun.Attributes;

namespace Platform.Geolocation.HostLocation.Stun
{
    /**
     *  0                   1                   2                   3
     *  0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
     * +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
     * |         Type                  |            Length             |
     * +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
     * |                         Value (variable)                ....
     * +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
     *
     * https://tools.ietf.org/html/rfc8489#section-14
     */
    public class StunAttribute
    {
        private static readonly Dictionary<StunAttributeType, Type> Handlers = new()
        {
            {
                StunAttributeType.SOFTWARE,
                typeof(StunAttributeSoftware)
            }
        };

        private byte[] _data = new byte[4]; // start with space for type and length

        public StunMessage Owner { get; private set; }

        public StunAttributeType Type
        {
            get => (StunAttributeType)((_data[0] << 8) | _data[1]);
            private init
            {
                _data[0] = (byte)(((ushort)value >> 8) & 0xFF);
                _data[1] = (byte)(((ushort)value >> 0) & 0xFF);
            }
        }

        private ushort Length
        {
            get => (ushort)((_data[2] << 8) | _data[3]);
            set
            {
                _data[2] = (byte)(((ushort)value >> 8) & 0xFF);
                _data[3] = (byte)(((ushort)value >> 0) & 0xFF);
            }
        }

        public ushort AttributeLength => (ushort)_data.Length;

        public byte[] Variable
        {
            get
            {
                var arr = new byte[Length];
                Array.Copy(_data, 4, arr, 0, arr.Length);
                return arr;
            }
            set
            {
                Length = (ushort)value.Length;
                
                // data must be aligned to 32-bit boundaries
                var padding = (4 - (Length % 4)) % 4;
                
                // resize to accommodate type, length,
                // variable and the alignment padding
                Array.Resize(ref _data, this.Length + padding + 4);
                Array.Copy(value, 0, this._data, 4, value.Length);
            }
        }

        public StunAttribute(Stream stream, StunMessage owner)
        {
            stream.Read(_data, 0, 4); // type and length
            var length = Length;
            var padding = (4 - (length % 4)) % 4;
            var variableLength = length + padding;
            Array.Resize(ref _data, variableLength + 4);
            stream.Read(_data, 4, variableLength);

            Owner = owner;
        }

        public StunAttribute(StunAttributeType type, StunMessage owner)
        {
            Type = type;
            Owner = owner;
        }

        public byte[] Serialize() => _data;

        public override string ToString() => $"{Type} {Length} bytes: {string.Join(",", Variable.Select(v => v.ToString("X2")))}";
    }
}