namespace EA.Weee.Domain
{
    using System;
    public class UKCompetentAuthority
    {
        protected UKCompetentAuthority()
        {
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Abbreviation { get; private set; }
 
        public string Region { get; private set; }
    }
}