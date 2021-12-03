namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.NonObligated;

    public class EditNonObligatedHandler : IRequestHandler<EditNonObligated, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly INonObligatedDataAccess nonObligatedDataAccess;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IMapWithParameter<EditNonObligated, Return, IEnumerable<Tuple<Guid, decimal?>>> mapper;

        public EditNonObligatedHandler(IWeeeAuthorization authorization, 
            INonObligatedDataAccess nonObligatedDataAccess,
            IReturnDataAccess returnDataAccess,
            IMapWithParameter<EditNonObligated, Return, IEnumerable<Tuple<Guid, decimal?>>> mapper)
        {
            this.authorization = authorization;
            this.nonObligatedDataAccess = nonObligatedDataAccess;
            this.mapper = mapper;
            this.returnDataAccess = returnDataAccess;
        }

        public async Task<bool> HandleAsync(EditNonObligated message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatfReturn = await returnDataAccess.GetById(message.ReturnId);

            var idsToAmounts = mapper.Map(message, aatfReturn);

            await nonObligatedDataAccess.UpdateNonObligatedWeeeAmounts(message.ReturnId, idsToAmounts);

            return true;
        }
    }
}
