namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf
{
    using System;
    using EA.Weee.Core.AatfReturn;

    public class OrganisationConfirmationViewModel
    {
        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public FacilityType FacilityType { get; set; }
    }
}