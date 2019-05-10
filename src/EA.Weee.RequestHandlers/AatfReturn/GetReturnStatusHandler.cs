namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Threading.Tasks;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;
    using ReturnStatus = Core.AatfReturn.ReturnStatus;

    internal class GetReturnStatusHandler : IRequestHandler<GetReturnStatus, ReturnStatus>
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

        public async Task<ReturnStatus> HandleAsync(GetReturnStatus message)
        {
            authorization.EnsureCanAccessExternalArea();

            var @return = await returnDataAccess.GetById(message.ReturnId);
            //SUBMIT RETURN HANDLER TESTS SHOULD COVER THIS / COPY
            if (@return == null)
            {
                throw new ArgumentException($"No return was found with id {message.ReturnId}.");
            }

            authorization.EnsureOrganisationAccess(@return.Operator.Organisation.Id);

            return mapper.Map<ReturnStatus>(@return.ReturnStatus);
        }
    }
}