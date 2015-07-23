using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
    public class ProducerSettings : IProducerBusinessSettings, IAuthorizedRepresentativeSettings
    {
        public SchemaVersion SchemaVersion { get; private set; }

        public ProducerSettings(SchemaVersion schemaVersion)
        {
            SchemaVersion = schemaVersion;
        }
    }
}
