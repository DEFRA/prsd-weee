namespace EA.Weee.Domain.AatfReturn
{
    using Prsd.Core.Domain;
    public abstract class ObligatedAmount : Entity, IObligatedAmount
    {
        public virtual decimal? HouseholdTonnage { get; set; }

        public virtual decimal? NonHouseholdTonnage { get; set; }

        public virtual void UpdateTonnages(decimal? houseHold, decimal? nonHouseHold)
        {
            HouseholdTonnage = houseHold;
            NonHouseholdTonnage = nonHouseHold;
        }
    }
}
