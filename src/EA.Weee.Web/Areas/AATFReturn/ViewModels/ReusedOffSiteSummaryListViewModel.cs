namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.AatfReturn;

    public class ReusedOffSiteSummaryListViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public string B2cTotal { get; set; }

        public string B2bTotal { get; set; }

        public List<AddressDataSummary> Addresses { get; set; }
    }
}