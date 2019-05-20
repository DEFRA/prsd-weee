namespace EA.Weee.Requests.Admin
{
    using EA.Weee.Core.AatfReturn;
    using Prsd.Core.Mediator;
    using System;

    public class AddAatf : IRequest<bool>
    {
        public AatfData Aatf { get; set; }
        public Guid OrganisationId { get; set; }
        public AatfContactData AatfContact { get; set; }
    }
}
