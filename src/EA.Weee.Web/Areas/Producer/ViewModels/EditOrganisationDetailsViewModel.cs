﻿namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using System;
    using System.Collections.Generic;

    public class EditOrganisationDetailsViewModel : IProducerSubmissionViewModel
    {
        public OrganisationViewModel Organisation { get; set; }

        public Guid DirectRegistrantId { get; set; }

        public Guid OrganisationId { get; set; }

        public List<AdditionalContactModel> AdditionalContactModels { get; set; }

        public bool HasAuthorisedRepresentitive { get; set; }

        public bool? RedirectToCheckAnswers { get; set; }
    }
}