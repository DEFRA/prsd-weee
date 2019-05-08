namespace EA.Weee.RequestHandlers.AatfReturn.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.AatfReturn.CheckYourReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.RequestHandlers.AatfReturn.Specification;
    using EA.Weee.Requests.AatfReturn.Internal;
    using Factories;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;
    using ReturnReportOn = Domain.AatfReturn.ReturnReportOn;

    public class GetContactHandler : IRequestHandler<GetContact, AatfContactData>
    {
        private readonly IWeeeAuthorization authorization;

        public GetContactHandler(IWeeeAuthorization authorization)
        {
            this.authorization = authorization;
        }

        public Task<AatfContactData> HandleAsync(GetContact message)
        {
            throw new NotImplementedException();
        }
    }
}
