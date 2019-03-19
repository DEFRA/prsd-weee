namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;

    public class AddObligatedReceived : ObligatedBaseRequest
    {
        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid SchemeId { get; set; }

        public Guid AatfId { get; set; }
    }
}
