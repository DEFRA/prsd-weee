﻿namespace EA.Weee.Core.Organisations
{
    using System;

    public class OrganisationTransactionData
    {
        public Guid? Id {get; set; }

        public string SearchTerm { get; set; }

        public TonnageType? TonnageType { get; set; }

        public ExternalOrganisationType? OrganisationType { get; set; }

        public YesNoType? PreviousRegistration { get; set; }

        public YesNoType? AuthorisedRepresentative { get; set; }

        // each screen view model
        public OrganisationDetails OrganisationDetails { get; set; }
    }
}
