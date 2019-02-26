namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.Obligated
{
    using DataAccess;
    using Domain.AatfReturn;
    using RequestHandlers.AatfReturn.ObligatedGeneric;

    public class ObligatedDataAccessTestAccess : ObligatedDataAccess<WeeeReceivedAmount>
    {
        public ObligatedDataAccessTestAccess(WeeeContext context) : base(context)
        {
        }
    }
}
