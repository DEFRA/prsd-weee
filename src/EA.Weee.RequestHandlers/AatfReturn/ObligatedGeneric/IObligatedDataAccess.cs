namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedGeneric
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;

    public interface IObligatedDataAccess<in T> where T : ObligatedAmount
    {
        Task Submit(IEnumerable<T> amounts);

        Task UpdateAmounts(T receivedAmount, decimal? houseHoldTonnage, decimal? nonHouseHoldTonnage);
    }
}
