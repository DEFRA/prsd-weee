namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;

    public class EditSentOnAatfSite : SentOnAatfSite
    {
        public Guid SiteAddressId { get; set; }

        public Guid OperatorAddressId { get; set; }

        public Guid WeeeSentOnId { get; set; }
    }
}
