namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedGeneric;

    public class ObligatedSentOnDataAccess : ObligatedDataAccess<WeeeSentOnAmount>, IObligatedSentOnDataAccess
    {
        public ObligatedSentOnDataAccess(WeeeContext context) : base(context)
        {
        }
    }
}
