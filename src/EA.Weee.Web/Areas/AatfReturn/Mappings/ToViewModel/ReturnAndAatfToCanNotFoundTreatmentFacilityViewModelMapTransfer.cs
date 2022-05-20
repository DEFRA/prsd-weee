namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ReturnAndAatfToCanNotFoundTreatmentFacilityViewModelMapTransfer
    {
        public ReturnAndAatfToCanNotFoundTreatmentFacilityViewModelMapTransfer()
        {
        }

        public ReturnData Return { get; set; }

        public Guid AatfId { get; set; }

        public string AatfName { get; set; }

        public bool IsCanNotFindLinkClick { get; set; }

        public WeeeSentOnData WeeeSentOnData { get; set; }

        public string SearchTerm { get; set; }

        public List<string> SearchResults { get; set; }
    }
}