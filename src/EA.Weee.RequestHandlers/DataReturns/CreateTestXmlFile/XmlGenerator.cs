namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using EA.Weee.Domain.DataReturns;

    public class XmlGenerator : IXmlGenerator
    {
        public XDocument GenerateXml(DataReturnContents dataReturnContents)
        {
            // TODO: Generate XML from the data return contents.

            return XDocument.Parse("<xml>TEST</xml>");
        }
    }
}
