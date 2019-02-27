namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using ObligatedGeneric;
    using ObligatedReceived;

    public class ObligatedReusedDataAccess : ObligatedDataAccess<WeeeReusedAmount>, IObligatedReusedDataAccess
    {
        public ObligatedReusedDataAccess(WeeeContext context) : base(context)
        {
        }
    }
}
