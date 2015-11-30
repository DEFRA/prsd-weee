namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Core.Exceptions;

    public class Deserializer : IDeserializer
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
