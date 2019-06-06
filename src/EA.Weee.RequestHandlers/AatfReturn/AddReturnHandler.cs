namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Organisation;
    using Domain.User;
    using Factories;
    using NonObligated;
    using Organisations;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Requests.AatfReturn.NonObligated;
    using Security;
    using Specification;
    using FacilityType = Core.AatfReturn.FacilityType;

    internal class AddReturnHandler : IRequestHandler<AddReturn, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IUserContext userContext;
        private readonly IReturnFactoryDataAccess returnFactoryDataAccess;

        public AddReturnHandler(IWeeeAuthorization authorization, 
            IReturnDataAccess returnDataAccess, 
            IGenericDataAccess genericDataAccess, 
            IUserContext userContext, 
            IReturnFactoryDataAccess returnFactoryDataAccess)
        {
            this.authorization = authorization;
            this.returnDataAccess = returnDataAccess;
            this.genericDataAccess = genericDataAccess;
            this.userContext = userContext;
            this.returnFactoryDataAccess = returnFactoryDataAccess;
        }

        public async Task<Guid> HandleAsync(AddReturn message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var existingReturn =
                await returnFactoryDataAccess.HasReturnQuarter(message.OrganisationId, message.Year, (QuarterType)message.Quarter, FacilityType.Aatf);

            if (existingReturn)
            {
                throw new InvalidOperationException("Return already exists");
            }

            var quarter = new Quarter(message.Year, (QuarterType)message.Quarter);

            var aatfOrganisation = await genericDataAccess.GetById<Organisation>(message.OrganisationId);

            var aatfReturn = new Return(aatfOrganisation, quarter, userContext.UserId.ToString());

            await returnDataAccess.Submit(aatfReturn);

            return aatfReturn.Id;
        }
    }
}
