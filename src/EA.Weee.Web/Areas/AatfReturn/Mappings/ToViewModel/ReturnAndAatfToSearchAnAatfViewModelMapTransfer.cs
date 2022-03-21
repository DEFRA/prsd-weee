namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ReturnAndAatfToSearchAnAatfViewModelMapTransfer
    {
        public ReturnAndAatfToSearchAnAatfViewModelMapTransfer()
        {
        }

        public ReturnData Return { get; set; }

        public Guid AatfId { get; set; }

        public WeeeSentOnData WeeeSentOnData { get; set; }
        
        public string SearchTerm { get; set; }

        public List<string> SearchResults { get; set; }
    }
}