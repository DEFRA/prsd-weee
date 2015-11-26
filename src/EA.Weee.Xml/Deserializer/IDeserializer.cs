namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System.Xml.Linq;

    public interface IDeserializer
    {
        T Deserialize<T>(XDocument document);
    } 
}
