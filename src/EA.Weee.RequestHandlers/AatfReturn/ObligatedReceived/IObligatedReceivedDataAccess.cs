namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;

    public interface IObligatedReceivedDataAccess
    {
        Task Submit(IEnumerable<WeeeReceivedAmount> aatfWeeReceivedAmount);

        Task UpdateAmounts(WeeeReceivedAmount receivedAmount, decimal? houseHoldTonnage, decimal? nonHouseHoldTonnage);
    }
}