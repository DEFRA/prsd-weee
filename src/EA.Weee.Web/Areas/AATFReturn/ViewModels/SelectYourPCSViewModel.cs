namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Validation;

    public class SelectYourPCSViewModel
    {
        public SelectYourPCSViewModel(List<SchemeData> schemeList, List<Guid> selectedSchemes)
        {
            Guard.ArgumentNotNull(() => schemeList, schemeList);
            Guard.ArgumentNotNull(() => selectedSchemes, selectedSchemes);

            this.SchemeList = schemeList;
            this.SelectedSchemes = selectedSchemes;
        }

        public SelectYourPCSViewModel()
        {
        }

        public List<SchemeData> SchemeList { get; set; }

        [MinimumElements(1, "You must select at least one PCS from the list")]
        public List<Guid> SelectedSchemes { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }
    }
}