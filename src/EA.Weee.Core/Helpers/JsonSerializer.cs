namespace EA.Weee.Core.Helpers
{
    using Newtonsoft.Json;

    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
