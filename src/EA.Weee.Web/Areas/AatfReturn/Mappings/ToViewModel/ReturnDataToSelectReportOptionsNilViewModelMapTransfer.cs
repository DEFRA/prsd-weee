namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System;
    using EA.Weee.Core.AatfReturn;

    public class ReturnDataToSelectReportOptionsNilViewModelMapTransfer
    {
        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }

        public ReturnData ReturnData { get; set; }
    }
}