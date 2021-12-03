namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Domain.AatfReturn;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn.NonObligated;
    using Security;

    public class AddNonObligatedHandler : IRequestHandler<AddNonObligated, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly INonObligatedDataAccess nonObligatedDataAccess;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IMapWithParameter<AddNonObligated, Return, IEnumerable<NonObligatedWeee>> mapper;

        public AddNonObligatedHandler(IWeeeAuthorization authorization,
            INonObligatedDataAccess nonObligatedDataAccess,
            IReturnDataAccess returnDataAccess,
            IMapWithParameter<AddNonObligated, Return, IEnumerable<NonObligatedWeee>> mapper)
        {
            this.authorization = authorization;
            this.nonObligatedDataAccess = nonObligatedDataAccess;
            this.returnDataAccess = returnDataAccess;
            this.mapper = mapper;
        }

        public async Task<bool> HandleAsync(AddNonObligated message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatfReturn = await returnDataAccess.GetById(message.ReturnId);

            var nonObligatedWeees = mapper.Map(message, aatfReturn);

            await nonObligatedDataAccess.InsertNonObligatedWeee(message.ReturnId, nonObligatedWeees);

            return true;
        }
    }
}
