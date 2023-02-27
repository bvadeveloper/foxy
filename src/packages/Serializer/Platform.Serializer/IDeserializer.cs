namespace Platform.Serializer;

public interface IDeserializer
{
    public T Deserialize<T>(byte[] value);
}