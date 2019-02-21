namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;

    public interface IAddObligatedReceivedDataAccess
    {
        Task Submit(IEnumerable<WeeeReceivedAmount> aatfWeeReceivedAmount);
    }
}