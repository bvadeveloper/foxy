using MemoryPack;

namespace Platform.Serializer.MemoryPack;

public class BytesSerializer : ISerializer
{
    public byte[] Serialize<T>(T value) => MemoryPackSerializer.Serialize(value);
}