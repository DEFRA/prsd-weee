namespace EA.Weee.Domain.DataReturns
{
    using EA.Weee.Domain.Lookup;

    public interface IReturnItem
    {
        ObligationType ObligationType { get; }

        WeeeCategory WeeeCategory { get; }

        decimal Tonnage { get; }
    }
}
