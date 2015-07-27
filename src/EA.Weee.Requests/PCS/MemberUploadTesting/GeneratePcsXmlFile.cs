namespace EA.Weee.Requests.PCS.MemberUploadTesting
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.PCS.MemberUploadTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GeneratePcsXmlFile : IRequest<PcsXmlFile>
    {
        public ProducerListSettings Settings { get; private set; }

        public GeneratePcsXmlFile(ProducerListSettings settings)
        {
            Settings = settings;
        }
    }
}
