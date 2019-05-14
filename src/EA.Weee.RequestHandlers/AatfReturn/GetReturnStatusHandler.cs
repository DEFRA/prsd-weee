namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;
    using ReturnStatus = Core.AatfReturn.ReturnStatus;

    internal class GetReturnStatusHandler : IRequestHandler<GetReturnStatus, ReturnData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IMapper mapper;

        public GetReturnStatusHandler(IWeeeAuthorization authorization, 
            IReturnDataAccess returnDataAccess, 
            IMapper mapper)
        {
            this.authorization = authorization;
            this.returnDataAccess = returnDataAccess;
            this.mapper = mapper;
        }

        public async Task<ReturnData> HandleAsync(GetReturnStatus message)
        {
            authorization.EnsureCanAccessExternalArea();

            var @return = await returnDataAccess.GetById(message.ReturnId);
            
            if (@return == null)
            {
                throw new ArgumentException($"No return was found with id {message.ReturnId}.");
            }

            authorization.EnsureOrganisationAccess(@return.Operator.Organisation.Id);

            var returnData = new ReturnData
            {
                ReturnStatus = mapper.Map<ReturnStatus>(@return.ReturnStatus),
                OrganisationId = @return.Operator.Organisation.Id
            };

            return returnData;
        }
    }
}