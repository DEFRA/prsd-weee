namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Collections.Generic;

    public class SentOnRemoveSiteViewModel : RadioButtonStringCollectionViewModel
    {
        public Guid WeeeSentOnId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public Guid OrganisationId { get; set; }

        public WeeeSentOnData WeeeSentOn { get; set; }

        public string SiteAddress { get; set; }

        public string OperatorAddress { get; set; }

        public decimal? TonnageB2B { get; set; }

        public decimal? TonnageB2C { get; set; }

        public ObligatedCategoryValue Tonnages { get; set; }

        public override string SelectedValue { get; set; }

        public SentOnRemoveSiteViewModel() : base(new List<string> { "Yes", "No" })
        {
        }
    }
}