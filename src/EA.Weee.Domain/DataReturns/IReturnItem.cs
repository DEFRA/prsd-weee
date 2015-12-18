namespace EA.Weee.Domain.DataReturns
{
    using Lookup;

    public interface IReturnItem
    {
        ObligationType ObligationType { get; }

        WeeeCategory WeeeCategory { get; }

        decimal Tonnage { get; }
    }
}
