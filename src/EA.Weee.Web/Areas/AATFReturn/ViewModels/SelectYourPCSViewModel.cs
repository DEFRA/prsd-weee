namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Validation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class SelectYourPCSViewModel
    {
        public SelectYourPCSViewModel(List<SchemeData> schemeList)
        {
            this.SchemeList = schemeList;
        }

        public SelectYourPCSViewModel()
        {
        }

        public List<SchemeData> SchemeList { get; set; }

        [EnsureOneElement(ErrorMessage = "You must select at least one PCS from the list")]
        public List<Guid> SelectedSchemes { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }
    }
}