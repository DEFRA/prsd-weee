namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using Domain.DataReturns;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Lookup;

    public class ReturnItem : IReturnItem
    {
        public WeeeCategory WeeeCategory { get; }

        public ObligationType ObligationType { get; }

        public decimal Tonnage { get; }

        public ReturnItem(WeeeCategory weeeCategory, ObligationType obligationtype, decimal tonnage)
        {
            WeeeCategory = weeeCategory;
            ObligationType = obligationtype;
            Tonnage = tonnage;
        }
    }
}
