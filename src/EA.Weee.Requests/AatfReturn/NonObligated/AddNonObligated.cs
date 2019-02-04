namespace EA.Weee.Requests.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core.Mediator;
    
    public class AddNonObligated : IRequest<bool>
    {
        public IList<NonObligatedValue> CategoryValues { get; set; }

        public bool Dcf { get; set; }

        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }
    }
}
