namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;

    public class EditSentOnAatfSiteWithOperator : AddSentOnAatfSite
    {
        public Guid? WeeeSentOnId { get; set; }
    }
}
