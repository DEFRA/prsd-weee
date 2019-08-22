namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using ObligatedGeneric;

    public class ObligatedReusedDataAccess : ObligatedDataAccess<WeeeReusedAmount>, IObligatedReusedDataAccess
    {
        public ObligatedReusedDataAccess(WeeeContext context) : base(context)
        {
        }
    }
}
