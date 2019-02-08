namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class SelectYourPCSViewModel
    {
        public SelectYourPCSViewModel(List<SchemeData> schemeList) {
            this.schemeList = schemeList;
        }

        public SelectYourPCSViewModel()
        {

        }

        public List<SchemeData> schemeList;
    }
}