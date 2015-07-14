﻿namespace EA.Weee.Core.Organisations
{
    using System;
    using Shared;

    public class OrganisationSearchData
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public AddressData Address { get; set; }
    }
}
