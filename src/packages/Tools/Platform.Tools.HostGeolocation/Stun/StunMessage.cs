using System.Collections.Immutable;

namespace Platform.Tools.HostGeolocation.Stun
{
    public class StunMessage
    {
        public readonly StunMessageHeader Header;
        public readonly List<StunAttribute> Attributes = new();

        public StunMessage() => Header = new StunMessageHeader();

        public StunMessage(Stream stream)
        {
            Header = new StunMessageHeader(stream);

            var pos = 0;
            while (pos < Header.Length)
            {
                var attribute = new StunAttribute(stream, this);
                pos += attribute.AttributeLength;
                Attributes.Add(attribute);
            }
        }

        public byte[] Serialize()
        {
            var serializedAttributes = Attributes.Select(a => a.Serialize()).ToImmutableArray();

            Header.Length = (ushort)serializedAttributes.Sum(a => a.Length);

            var message = new byte[Header.Length + 20];
            var curIndex = 20;

            foreach (var attr in serializedAttributes)
            {
                Array.Copy(attr, 0, message, curIndex, attr.Length);
                curIndex += attr.Length;
            }

            var header = Header.Serialize();
            Array.Copy(header, 0, message, 0, 20);
            
            return message;
        }

        public override string ToString() => $"{Header}\n{string.Join("\n", Attributes.Select(a => a.ToString()))}";
    }
}