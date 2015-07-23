using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
    public class ProducerListSettings : ISchemeBusinessSettings
    {
        public SchemaVersion SchemaVersion { get; private set; }

        public int NumberOfNewProducers { get; set; }

        public ProducerListSettings(SchemaVersion schemaVersion, int numberOfNewProducers)
        {
            SchemaVersion = schemaVersion;
            NumberOfNewProducers = numberOfNewProducers;
        }
    }
}
