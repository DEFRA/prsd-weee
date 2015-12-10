namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using EA.Weee.Domain.DataReturns;

    public interface IXmlGenerator
    {
        XDocument GenerateXml(DataReturnVersion dataReturnVersion);
    }
}
