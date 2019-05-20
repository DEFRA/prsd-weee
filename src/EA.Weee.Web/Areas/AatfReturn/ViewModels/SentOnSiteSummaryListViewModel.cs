namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;

    public class SentOnSiteSummaryListViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public String AatfName { get; set; }

        public List<WeeeSentOnSummaryListData> Sites { get; set; }

        public ObligatedCategoryValue Tonnages { get; set; }

        public SentOnSiteSummaryListViewModel()
        {
        }
    }
}