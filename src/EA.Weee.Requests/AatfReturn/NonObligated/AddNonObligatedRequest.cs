namespace EA.Weee.Requests.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core.Mediator;
    
    public class AddNonObligatedRequest : IRequest<bool>
    {
        public IList<NonObligatedRequestValue> CategoryValues { get; set; }

        public bool Dcf { get; set; }

        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }
    }
}
