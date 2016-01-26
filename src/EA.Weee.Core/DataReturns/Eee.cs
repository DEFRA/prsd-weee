namespace EA.Weee.Core.DataReturns
{
    using Shared;

    public class Eee
    {
        public decimal Tonnage { get; private set; }

        public WeeeCategory Category { get; private set; }

        public ObligationType ObligationType { get; private set; }

        public Eee(decimal tonnage, WeeeCategory category, ObligationType obligationType)
        {
            Tonnage = tonnage;
            Category = category;
            ObligationType = obligationType;
        }
    }
}
