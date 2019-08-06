namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class ViewAatfContactDetailsViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public ViewAatfContactDetailsViewModel()
        {
        }
    }
}