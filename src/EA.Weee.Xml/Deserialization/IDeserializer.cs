namespace EA.Weee.Xml.Deserialization
{
    using System.Xml.Linq;

    public interface IDeserializer
    {
        T Deserialize<T>(XDocument document);
    } 
}
