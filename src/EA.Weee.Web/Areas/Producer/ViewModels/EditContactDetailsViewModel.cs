﻿namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.Organisations;
    using System;

    public class EditContactDetailsViewModel
    {
        public ContactDetailsViewModel ContactDetails { get; set; }

        public Guid DirectRegistrantId { get; set; }

        public Guid OrganisationId { get; set; }

        public bool HasAuthorisedRepresentitive { get; set; }
    }
}