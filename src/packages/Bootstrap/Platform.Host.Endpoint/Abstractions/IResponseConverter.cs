namespace Platform.Host.Endpoint.Abstractions
{
    public interface IResponseConverter
    {
        T Deserialize<T>(string value);

        string SerializeObject(object obj);
    }
}