namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;

    public class AddObligatedSentOn : ObligatedBaseRequest
    {
        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public Guid SiteAddressId { get; set; }

        public Guid WeeeSentOnId { get; set; }
    }
}
