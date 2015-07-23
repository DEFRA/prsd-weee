using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
    public class XmlGeneratorFactory
    {
        public IXmlGenerator CreateGenerator(SchemaVersion schemaVersion)
        {
            switch (schemaVersion)
            {
                case SchemaVersion.Version_3_04:
                case SchemaVersion.Version_3_06:
                    return new XmlGenerator306();

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
