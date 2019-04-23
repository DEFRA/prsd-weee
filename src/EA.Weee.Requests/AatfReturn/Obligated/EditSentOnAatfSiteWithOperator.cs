namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;

    public class EditSentOnAatfSiteWithOperator : SentOnAatfSite
    {
        public Guid? WeeeSentOnId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }
    }
}
