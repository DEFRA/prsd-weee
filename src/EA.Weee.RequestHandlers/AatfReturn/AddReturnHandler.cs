namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using NonObligated;
    using Organisations;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Requests.AatfReturn.NonObligated;
    using Security;
    using Specification;

    internal class AddReturnHandler : IRequestHandler<AddReturn, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly IGenericDataAccess operatorAccess;

        public AddReturnHandler(IWeeeAuthorization authorization, 
            IReturnDataAccess returnDataAccess, 
            IOrganisationDataAccess organisationDataAccess, 
            IGenericDataAccess operatorAccess)
        {
            this.authorization = authorization;
            this.returnDataAccess = returnDataAccess;
            this.organisationDataAccess = organisationDataAccess;
            this.operatorAccess = operatorAccess;
        }

        public async Task<Guid> HandleAsync(AddReturn message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var quarter = new Quarter(2019, QuarterType.Q1);

            var organisation = await organisationDataAccess.GetById(message.OrganisationId);

            var aatfOperator = await operatorAccess.GetSingleByExpression<Operator>(new OperatorByOrganisationIdSpecification(message.OrganisationId));

            var aatfReturn = new Return(aatfOperator, quarter, ReturnStatus.Created);

            await returnDataAccess.Submit(aatfReturn);

            return aatfReturn.Id;
        }
    }
}
