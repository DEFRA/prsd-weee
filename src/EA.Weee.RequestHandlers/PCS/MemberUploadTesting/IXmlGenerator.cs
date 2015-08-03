namespace EA.Weee.RequestHandlers.PCS.MemberUploadTesting
{
    using EA.Weee.Core.PCS.MemberUploadTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Creates an XML Document that can be used for testing the PCS member upload functionality
    /// based on the content of a ProducerList.
    /// </summary>
    public interface IXmlGenerator
    {
        XDocument GenerateXml(ProducerList producerList, ProducerListSettings settings);
    }
}
