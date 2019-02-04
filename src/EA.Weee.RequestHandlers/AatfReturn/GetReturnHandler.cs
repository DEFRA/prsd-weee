namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;

    internal class GetReturnHandler : IRequestHandler<GetReturn, ReturnData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly IMap<Return, ReturnData> mapper;

        public GetReturnHandler(IWeeeAuthorization authorization, 
            IReturnDataAccess returnDataAccess, 
            IOrganisationDataAccess organisationDataAccess, 
            IMap<Return, ReturnData> mapper)
        {
            this.authorization = authorization;
            this.returnDataAccess = returnDataAccess;
            this.organisationDataAccess = organisationDataAccess;
            this.mapper = mapper;
        }

        public async Task<ReturnData> HandleAsync(GetReturn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var @return = await returnDataAccess.GetById(message.ReturnId);

            authorization.EnsureOrganisationAccess(@return.Operator.Organisation.Id);

            return mapper.Map(@return);
        }
    }
}
