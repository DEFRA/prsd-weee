namespace EA.Weee.Xml.Deserializer
{
    using System.Xml.Linq;

    public interface IDeserializer
    {
        T Deserialize<T>(XDocument document);
    } 
}
