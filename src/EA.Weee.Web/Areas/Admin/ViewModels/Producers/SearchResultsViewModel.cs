namespace EA.Weee.Web.Areas.Admin.ViewModels.Producers
{
    using EA.Weee.Core.Admin;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class SearchResultsViewModel
    {
        public IList<ProducerSearchResult> Results { get; set; }
    }
}