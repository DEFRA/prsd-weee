namespace EA.Weee.Api.Client.Serlializer
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(string json);
    }
}
