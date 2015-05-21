namespace EA.Weee.Domain
{
    using Prsd.Core.Domain;

    public class Country : Entity
    {
        private Country()
        {
        }

        public string Name { get; private set; }

        public string IsoAlpha2Code { get; private set; }

        public bool IsEuropeanUnionMember { get; private set; }
    }
}