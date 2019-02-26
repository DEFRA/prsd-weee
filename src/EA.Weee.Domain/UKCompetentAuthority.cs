namespace EA.Weee.Domain
{
    using System;
    public class UKCompetentAuthority
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Abbreviation { get; private set; }

        public virtual Country Country { get; private set; }

        public virtual string Email { get; private set; }

        public virtual decimal? AnnualChargeAmount { get; set; }

        protected UKCompetentAuthority()
        {
        }

        public UKCompetentAuthority(Guid id, string name, string abbreviation, Country country, string email, decimal? annualChargeAmount)
        {
            Id = id;
            Name = name;
            Abbreviation = abbreviation;
            Country = country;
            Email = email;
            AnnualChargeAmount = annualChargeAmount;
        }
    }
}