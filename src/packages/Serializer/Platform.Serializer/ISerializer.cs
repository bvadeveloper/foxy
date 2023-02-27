namespace Platform.Serializer;

public interface ISerializer
{
    public byte[] Serialize<T>(T value);
}