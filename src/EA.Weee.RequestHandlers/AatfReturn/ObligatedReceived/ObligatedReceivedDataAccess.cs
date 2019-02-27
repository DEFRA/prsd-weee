namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived
{
    using DataAccess;
    using Domain.AatfReturn;
    using ObligatedGeneric;

    public class ObligatedReceivedDataAccess : ObligatedDataAccess<WeeeReceivedAmount>, IObligatedReceivedDataAccess
    {
        public ObligatedReceivedDataAccess(WeeeContext context) : base(context)
        {
        }
    }
}
