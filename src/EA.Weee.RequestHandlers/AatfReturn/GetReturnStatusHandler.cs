namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;
    using ReturnStatus = Core.AatfReturn.ReturnStatus;

    internal class GetReturnStatusHandler : IRequestHandler<GetReturnStatus, ReturnStatusData>
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

        public async Task<ReturnStatusData> HandleAsync(GetReturnStatus message)
        {
            authorization.EnsureCanAccessExternalArea();

            var @return = await returnDataAccess.GetById(message.ReturnId);
            
            if (@return == null)
            {
                throw new ArgumentException($"No return was found with id {message.ReturnId}.");
            }

            authorization.EnsureOrganisationAccess(@return.Operator.Organisation.Id);

            var returnsInYear = await returnDataAccess.GetByComplianceYearAndQuarter(@return);

            var returnData = new ReturnStatusData
            {
                ReturnStatus = mapper.Map<ReturnStatus>(@return.ReturnStatus),
                OrganisationId = @return.Operator.Organisation.Id,
                OtherInProgressReturn = returnsInYear.Any(r => r.ReturnStatus == EA.Weee.Domain.AatfReturn.ReturnStatus.Created)
            };

            return returnData;
        }
    }
}