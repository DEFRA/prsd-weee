namespace EA.Weee.Core.Organisations
{
    using System;

    public class OrganisationTransactionData
    {
        public Guid Id {get; set; }

        public bool? PreviousNpwdAccount { get; set; }

        // each screen view model
        public OrganisationDetails OrganisationDetails { get; set; }
    }
}
