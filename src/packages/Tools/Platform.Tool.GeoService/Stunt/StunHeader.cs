using System.Security.Cryptography;

namespace Platform.Tool.GeoService.Stunt
{
    public enum StunMessageClass : ushort
    {
        Request = 0b00,
        Indication = 0b01,
        Success = 0b10,
        Error = 0b11,
    }

    public enum StunMessageMethod : ushort
    {
        Binding = 0x001,
    }

    public class StunMessageHeader
    {
        public const uint MAGIC_COOKIE = 0x2112A442;

        private readonly byte[] _data = new byte[20];

        public ushort Type
        {
            get => (ushort)((_data[0] << 8) | _data[1]);
            set
            {
                _data[0] = (byte)((value >> 8) & 0xFF);
                _data[1] = (byte)((value >> 0) & 0xFF);
            }
        }

        public ushort Length
        {
            get => (ushort)((_data[2] << 8) | _data[3]);
            set
            {
                _data[2] = (byte)((value >> 8) & 0xFF);
                _data[3] = (byte)((value >> 0) & 0xFF);
            }
        }

        public StunMessageClass Class
        {
            get => (StunMessageClass)(
                (Type & 0x0100) >> 7 |
                (Type & 0x0010) >> 4
            );
            set => Type = (ushort)((ushort)value | (ushort)Method);
        }

        public StunMessageMethod Method
        {
            get => (StunMessageMethod)(
                (Type & 0x3E00) >> 2 |
                (Type & 0x00E0) >> 1 |
                (Type & 0x000F)
            );
            set => Type = (ushort)((ushort)value | (ushort)Class);
        }

        public string TransactionIdBase64
        {
            get => Convert.ToBase64String(_data, 8, 96 / 8);
        }

        public byte[] TransactionId
        {
            get
            {
                var arr = new byte[96 / 8];
                Array.Copy(_data, 8, arr, 0, arr.Length);
                return arr;
            }
        }

        public StunMessageHeader()
        {
            _data[4] = (byte)((MAGIC_COOKIE >> 24) & 0xFF);
            _data[5] = (byte)((MAGIC_COOKIE >> 16) & 0xFF);
            _data[6] = (byte)((MAGIC_COOKIE >> 8) & 0xFF);
            _data[7] = (byte)((MAGIC_COOKIE >> 0) & 0xFF);

            Class = StunMessageClass.Request;
            Method = StunMessageMethod.Binding;
            
            RandomNumberGenerator.Create().GetBytes(_data, 8, 12); // random transaction id
        }

        public StunMessageHeader(Stream stream)
        {
            stream.Read(_data, 0, 20);

            if ((Type & 0xC000) != 0)
            {
                throw new ArgumentException("Header must start with two padding zeroes", nameof(stream));
            }

            var magicCookie = (_data[4] << 24) | (_data[5] << 16) | (_data[6] << 8) | _data[7];

            if (magicCookie != MAGIC_COOKIE)
            {
                throw new ArgumentException($"Magic cookie doesn't match, expected 0x{MAGIC_COOKIE:X4} got 0x{magicCookie:X4}", nameof(stream));
            }
        }

        public byte[] Serialize() => _data;

        public override string ToString() => $"StunHeader of type {Type:X4}, class {Class}, method {Method}, and {Length} bytes of message";
    }
}