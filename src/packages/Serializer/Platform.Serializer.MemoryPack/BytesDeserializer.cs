using MemoryPack;

namespace Platform.Serializer.MemoryPack;

public class BytesDeserializer : IDeserializer
{
    public T Deserialize<T>(byte[] value) => MemoryPackSerializer.Deserialize<T>(value);
}