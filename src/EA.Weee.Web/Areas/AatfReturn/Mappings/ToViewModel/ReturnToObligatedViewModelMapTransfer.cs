namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System;
    using Core.AatfReturn;

    public class ReturnToObligatedViewModelMapTransfer
    {
        public ReturnData ReturnData { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid SchemeId { get; set; }

        public string SiteName { get; set; }

        public Guid AatfId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid WeeeSentOnId { get; set; }

        public ObligatedCategoryValue PastedData { get; set; }

        public ReturnToObligatedViewModelMapTransfer()
        {
            ReturnData = new ReturnData();
        }
    }
}