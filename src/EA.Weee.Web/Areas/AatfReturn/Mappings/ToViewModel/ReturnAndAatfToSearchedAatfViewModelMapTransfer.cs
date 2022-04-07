namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;

    public class ReturnAndAatfToSearchedAatfViewModelMapTransfer
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public string AatfName { get; set; }

        public Guid? SelectedWeeeSentOnId { get; set; }

        public Guid SelectedAatfId { get; set; }

        public string SelectedAatfName { get; set; }

        public List<WeeeSearchedAnAatfListData> Sites { get; set; }
    }
}