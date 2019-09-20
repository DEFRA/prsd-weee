namespace EA.Weee.RequestHandlers.AatfReturn
{
    using Core.AatfReturn;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;
    using System.Threading.Tasks;

    internal class GetReturnHandler : IRequestHandler<GetReturn, ReturnData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetPopulatedReturn getPopulatedReturn;

        public GetReturnHandler(IWeeeAuthorization authorization,
            IGetPopulatedReturn getPopulatedReturn)
        {
            this.authorization = authorization;
            this.getPopulatedReturn = getPopulatedReturn;
        }

        public async Task<ReturnData> HandleAsync(GetReturn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var returnData = await getPopulatedReturn.GetReturnData(message.ReturnId, message.ForSummary);

            return returnData;
        }
    }
}