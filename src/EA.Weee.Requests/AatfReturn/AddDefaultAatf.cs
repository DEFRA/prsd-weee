namespace EA.Weee.Requests.AatfReturn
{
    using Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;    

    public class AddDefaultAatf : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }
    }
}
