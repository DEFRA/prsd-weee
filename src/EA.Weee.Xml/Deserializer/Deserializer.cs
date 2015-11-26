namespace EA.Weee.Xml.Deserializer
{
    using System;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    internal class Deserializer : IDeserializer
    {
        public T Deserialize<T>(XDocument document)
        {
            try
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(document.CreateReader());
            }
            catch (InvalidOperationException ex)
            {
                throw new XmlDeserializationFailureException(ex);
            }
        }
    }
}
