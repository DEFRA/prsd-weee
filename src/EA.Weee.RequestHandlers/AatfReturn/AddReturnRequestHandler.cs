namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using NonObligated;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Requests.AatfReturn.NonObligated;
    using Security;

    internal class AddReturnRequestHandler : IRequestHandler<AddReturnRequest, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAddNonObligatedDataAccess nonObligatedDataAccess;

        public AddReturnRequestHandler(IWeeeAuthorization authorization, IAddNonObligatedDataAccess nonObligatedDataAccess)
        {
            this.authorization = authorization;
            this.nonObligatedDataAccess = nonObligatedDataAccess;
        }

        public async Task<Guid> HandleAsync(AddReturnRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            
            return Guid.NewGuid();
        }
    }
}
