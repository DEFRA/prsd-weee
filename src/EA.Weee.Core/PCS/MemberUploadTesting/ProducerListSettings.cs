using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
    public class ProducerListSettings : ISchemeBusinessSettings
    {
        public Guid OrganisationID { get; set; }

        public SchemaVersion SchemaVersion { get; set; }

        public int ComplianceYear { get; set; }

        public int NumberOfNewProducers { get; set; }

        public int NumberOfExistingProducers { get; set; }

        public bool IncludeMalformedSchema { get; set; }

        public bool IncludeUnexpectedFooElement { get; set; }
    }
}
