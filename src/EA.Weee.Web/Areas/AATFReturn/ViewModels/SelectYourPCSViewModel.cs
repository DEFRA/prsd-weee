namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.Scheme;
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

        public List<Guid> SelectedSchemes { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }
    }
}