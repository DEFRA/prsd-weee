namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class SentOnRemoveSiteViewModel : RadioButtonStringCollectionViewModel
    {
        public Guid WeeeSentOnId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public Guid OrganisationId { get; set; }

        public WeeeSentOnData WeeeSentOn { get; set; }

        [AllowHtml]
        public string SiteAddress { get; set; }

        [AllowHtml]
        public string OperatorAddress { get; set; }

        public string TonnageB2B { get; set; }

        public string TonnageB2C { get; set; }

        public ObligatedCategoryValue Tonnages { get; set; }

        public override string SelectedValue { get; set; }

        public SentOnRemoveSiteViewModel() : base(new List<string> { "Yes", "No" })
        {
        }
    }
}