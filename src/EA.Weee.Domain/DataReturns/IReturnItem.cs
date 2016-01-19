namespace EA.Weee.Domain.DataReturns
{
    using Lookup;
    using Obligation;
    public interface IReturnItem
    {
        ObligationType ObligationType { get; }

        WeeeCategory WeeeCategory { get; }

        decimal Tonnage { get; }
    }
}
