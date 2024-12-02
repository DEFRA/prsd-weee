namespace EA.Weee.Core.Organisations
{
    using Shared;
    using System;

    public class PublicOrganisationData
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public AddressData Address { get; set; }

        public bool NpwdMigratedComplete { get; set; }

        public bool NpwdMigrated { get; set; }
    }
}
