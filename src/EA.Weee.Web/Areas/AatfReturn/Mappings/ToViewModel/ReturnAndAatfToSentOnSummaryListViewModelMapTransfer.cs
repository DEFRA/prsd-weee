namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;

    public class ReturnAndAatfToSentOnSummaryListViewModelMapTransfer
    {
        public string AatfName { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public virtual AatfAddressData OperatorAddress { get; set; }

        public virtual AatfAddressData SiteAddress { get; set; }

        public List<WeeeObligatedData> WeeeObligatedData { get; set; }

        public List<WeeeSentOnData> WeeeSentOnDataItems { get; set; }
    }
}