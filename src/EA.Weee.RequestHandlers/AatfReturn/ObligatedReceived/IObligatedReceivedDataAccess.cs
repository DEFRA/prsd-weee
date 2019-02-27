namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using ObligatedGeneric;

    public interface IObligatedReceivedDataAccess : IObligatedDataAccess<WeeeReceivedAmount>
    {
    }
}