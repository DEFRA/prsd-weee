namespace EA.Weee.RequestHandlers.Scheme.MemberUploadTesting
{
    using System.Xml.Linq;
    using Core.Scheme.MemberUploadTesting;

    /// <summary>
    /// Creates an XML Document that can be used for testing the PCS member upload functionality
    /// based on the content of a ProducerList.
    /// </summary>
    public interface IXmlGenerator
    {
        XDocument GenerateXml(ProducerList producerList, ProducerListSettings settings);
    }
}
