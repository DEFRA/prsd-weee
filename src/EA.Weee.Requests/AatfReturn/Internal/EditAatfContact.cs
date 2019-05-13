namespace EA.Weee.Requests.AatfReturn.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;

    public class EditAatfContact : IRequest<bool>
    {
        public AatfContactData ContactData { get; set; }
    }
}
