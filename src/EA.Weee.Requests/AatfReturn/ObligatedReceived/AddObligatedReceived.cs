namespace EA.Weee.Requests.AatfReturn.ObligatedReceived
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core.Mediator;

    public class AddObligatedReceived : IRequest<bool>
    {
        public IList<ObligatedReceivedValue> CategoryValues { get; set; }

        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }
    }
}
