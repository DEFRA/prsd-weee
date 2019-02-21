namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core.Mediator;

    public class AddObligatedReused : IRequest<bool>
    {
        public IList<ObligatedValue> CategoryValues { get; set; }

        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }
    }
}
