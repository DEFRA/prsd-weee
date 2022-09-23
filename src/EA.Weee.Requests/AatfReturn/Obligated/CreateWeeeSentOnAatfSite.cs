namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;

    public class CreateWeeeSentOnAatfSite : SentOnAatfSite
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public Guid SelectedAatfId { get; set; }

        public Guid WeeeSentOnId { get; set; }
    }
}
