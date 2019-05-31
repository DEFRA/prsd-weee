namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using System;

    public class ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer
    {
        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public Guid OrganisationId { get; set; }

        public WeeeSentOnData WeeeSentOn { get; set; }

        public string SelectedValue { get; set; }

        public ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer()
        {
        }
    }
}