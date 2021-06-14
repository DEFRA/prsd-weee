namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
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
        //private readonly INonObligatedObjectMapper mapper;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IMapWithParameter<IEnumerable<NonObligatedValue>, Return, IEnumerable<NonObligatedWeee>> mapper;

        public EditNonObligatedHandler(IWeeeAuthorization authorization, 
            INonObligatedDataAccess nonObligatedDataAccess,
            IMapWithParameter<IEnumerable<NonObligatedValue>, Return, IEnumerable<NonObligatedWeee>> mapper, 
            IReturnDataAccess returnDataAccess)
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

            //var nonObligatedWeees = mapper.MapEditNonObligated(message, aatfReturn);
            var nonObligatedWeees = mapper.Map(message.CategoryValues, aatfReturn);

            await nonObligatedDataAccess.AddUpdateAndClean(message.ReturnId, nonObligatedWeees);

            return true;
        }
    }
}
