namespace EA.Prsd.Core.Web.Converters
{
    using System;
    using Domain;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class EnumerationConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsSubclassOf(typeof(Enumeration));
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            // Passed as a parameter to retrieve the enum value using the FromValue<T> method.
            // The double cast is required to create a parameter of the correct type as an object.
            var parameter = (object)(int)jsonObject["Value"];

            // Create an instance from the protected empty constructor in order to invoke the method using reflection.
            var blankEnumeration = Activator.CreateInstance(objectType, true);

            // Get the method and convert it to a generic method using the type we need.
            var method = typeof(Enumeration).GetMethod("FromValue");
            var genericMethod = method.MakeGenericMethod(objectType);

            return genericMethod.Invoke(blankEnumeration, new[] { parameter });
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
