namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.AatfReturn;

    internal class GetWeeeReusedByReturnId : IRequestHandler<GetObligatedReused, ObligatedData> 
    {
        private readonly WeeeContext context;
        private readonly IGenericDataAccess genericDataAccess;

        public GetWeeeReusedByReturnId(IGenericDataAccess genericDataAccess,
            WeeeContext context)
        {
            this.genericDataAccess = genericDataAccess;
            this.context = context;
        }

        public async Task<ObligatedData> HandleAsync(GetObligatedReused message)
        {
           genericDataAccess.GetById
        }
    }
}
