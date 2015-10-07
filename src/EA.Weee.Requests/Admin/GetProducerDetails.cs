namespace EA.Weee.Requests.Admin
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetProducerDetails : IRequest<ProducerDetails>
    {
        public string RegistrationNumber { get; set; }
    }
}
