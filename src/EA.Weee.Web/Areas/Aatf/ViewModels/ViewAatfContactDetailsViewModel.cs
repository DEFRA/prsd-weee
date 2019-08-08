namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using System;

    public class ViewAatfContactDetailsViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public string AatfName { get; set; }

        public AatfContactData Contact { get; set; }

        public bool IsAE { get; set; }

        public ViewAatfContactDetailsViewModel()
        {
        }
    }
}