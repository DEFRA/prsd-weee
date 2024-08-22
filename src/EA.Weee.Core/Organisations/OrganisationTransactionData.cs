namespace EA.Weee.Core.Organisations
{
    using System;

    public class OrganisationTransactionData
    {
        public Guid? Id {get; set; }

        public string PreviousRegistration { get; set; }

        // each screen view model
        public OrganisationDetails OrganisationDetails { get; set; }
    }
}
