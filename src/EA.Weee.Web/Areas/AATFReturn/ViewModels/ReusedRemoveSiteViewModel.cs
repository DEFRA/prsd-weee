namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Collections.Generic;

    public class ReusedRemoveSiteViewModel : RadioButtonStringCollectionViewModel
    {
        public Guid SiteId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public Guid OrganisationId { get; set; }

        public AddressData Site { get; set; }

        public string SiteAddress { get; set; }

        public override string SelectedValue { get; set; }

        public ReusedRemoveSiteViewModel() : base(new List<string> { "Yes", "No" })
        {
        }
    }
}