namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using EA.Weee.Web.ViewModels.Shared;

    public class RepresentedCompaniesViewModel : RadioButtonStringCollectionViewModel
    {
        public Guid OrganisationId { get; set; }
    }
}