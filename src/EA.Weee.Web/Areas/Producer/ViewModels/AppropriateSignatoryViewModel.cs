namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Web.ViewModels.Shared;
    using System;

    public class AppropriateSignatoryViewModel
    {
        public ContactPersonViewModel Contact { get; set; }

        public Guid DirectRegistrantId { get; set; }

        public Guid OrganisationId { get; set; }

        public bool HasAuthorisedRepresentitive { get; set; }
    }
}