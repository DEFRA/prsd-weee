namespace EA.Weee.RequestHandlers.AatfReturn
{
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Organisation;
    using Factories;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;
    using System;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    internal class AddReturnHandler : IRequestHandler<AddReturn, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IUserContext userContext;
        private readonly IReturnFactoryDataAccess returnFactoryDataAccess;
        private readonly IReturnFactory returnFactory;

        public AddReturnHandler(IWeeeAuthorization authorization,
            IReturnDataAccess returnDataAccess,
            IGenericDataAccess genericDataAccess,
            IUserContext userContext,
            IReturnFactoryDataAccess returnFactoryDataAccess,
            IReturnFactory returnFactory)
        {
            this.authorization = authorization;
            this.returnDataAccess = returnDataAccess;
            this.genericDataAccess = genericDataAccess;
            this.userContext = userContext;
            this.returnFactoryDataAccess = returnFactoryDataAccess;
            this.returnFactory = returnFactory;
        }

        public async Task<Guid> HandleAsync(AddReturn message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            await ValidateReturnMessage(message);

            var quarter = new Quarter(message.Year, (QuarterType)message.Quarter);

            var aatfOrganisation = await genericDataAccess.GetById<Organisation>(message.OrganisationId);

            var type = Enumeration.FromValue<EA.Weee.Domain.AatfReturn.FacilityType>((int)message.FacilityType);

            var aatfReturn = new Return(aatfOrganisation, quarter, userContext.UserId.ToString(), type);

            await returnDataAccess.Submit(aatfReturn);

            return aatfReturn.Id;
        }

        private async Task ValidateReturnMessage(AddReturn message)
        {
            var existingReturn =
                await returnFactoryDataAccess.HasReturnQuarter(message.OrganisationId, message.Year, (QuarterType)message.Quarter, message.FacilityType);

            var returnWindow = await returnFactory.GetReturnQuarter(message.OrganisationId, message.FacilityType);

            if (existingReturn || returnWindow.Q != message.Quarter || returnWindow.Year != message.Year)
            {
                throw new InvalidOperationException("Return already exists");
            }
        }
    }
}
