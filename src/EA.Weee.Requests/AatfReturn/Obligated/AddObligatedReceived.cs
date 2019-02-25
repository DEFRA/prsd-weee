namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core.Mediator;

    public class AddObligatedReceived : ObligatedReceived
    {
        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid SchemeId { get; set; }

        public Guid AatfId { get; set; }
    }
}
