using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
    public interface IXmlGenerator
    {
        XDocument GenerateXml(ProducerList producerList);
    }
}
