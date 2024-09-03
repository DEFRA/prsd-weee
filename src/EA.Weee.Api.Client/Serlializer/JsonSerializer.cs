namespace EA.Weee.Api.Client.Serlializer
{
    using System.Text.Json;

    public class JsonSerializer : IJsonSerializer
    {
        public T Deserialize<T>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions());
        }
    }
}
