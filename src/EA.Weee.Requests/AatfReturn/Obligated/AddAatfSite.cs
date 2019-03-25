namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;

    public class AddAatfSite : AatfSite
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }
    }
}