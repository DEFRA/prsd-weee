﻿namespace EA.Weee.Core.Shared
{
    using System;

    [Serializable]
    public class UKCompetentAuthorityData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Abbreviation { get; set; }

        public Guid CountryId { get; set; }

        public string Email { get; set; }

        public decimal? AnnualChargeAmount { get; set; }

        public bool Selected { get; set; }
    }
}