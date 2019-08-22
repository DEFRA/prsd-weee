namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using System;

    public class ReturnDataToSelectReportOptionsNilViewModelMapTransfer
    {
        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }

        public ReturnData ReturnData { get; set; }
    }
}