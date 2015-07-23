namespace EA.Weee.Requests.PCS.MemberUploadTesting
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.PCS.MemberUploadTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GeneratePcsXml : IRequest<GeneratedXmlFile>
    {
        public ProducerListSettings Settings { get; private set; }

        public GeneratePcsXml(ProducerListSettings settings)
        {
            Settings = settings;
        }
    }
}
