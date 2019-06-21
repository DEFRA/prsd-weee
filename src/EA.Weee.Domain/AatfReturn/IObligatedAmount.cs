namespace EA.Weee.Domain.AatfReturn
{
    public interface IObligatedAmount
    {
        decimal? HouseholdTonnage { get; set; }

        decimal? NonHouseholdTonnage { get; set; }
    }
}
