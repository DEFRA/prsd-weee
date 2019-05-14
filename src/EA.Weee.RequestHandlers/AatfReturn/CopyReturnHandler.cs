namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Threading.Tasks;
    using CheckYourReturn;
    using Core.AatfReturn;
    using NonObligated;
    using Prsd.Core.Domain;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;
    using ReturnStatus = Core.AatfReturn.ReturnStatus;

    internal class CopyReturnHandler : IRequestHandler<CopyReturn, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IUserContext userContext;
        private readonly IGetPopulatedReturn getPopulatedReturn;
        private readonly INonObligatedDataAccess fetchNonObligatedWeeeForReturnDataAccess;

        public CopyReturnHandler(IWeeeAuthorization authorization, 
            IReturnDataAccess returnDataAccess, 
            IUserContext userContext, 
            IGetPopulatedReturn getPopulatedReturn,
            INonObligatedDataAccess fetchNonObligatedWeeeForReturnDataAccess)
        {
            this.authorization = authorization;
            this.returnDataAccess = returnDataAccess;
            this.userContext = userContext;
            this.getPopulatedReturn = getPopulatedReturn;
            this.fetchNonObligatedWeeeForReturnDataAccess = fetchNonObligatedWeeeForReturnDataAccess;
        }

        public async Task<bool> HandleAsync(CopyReturn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var @return = await returnDataAccess.GetById(message.ReturnId);

            var returnCopy = await returnDataAccess.GetByIdWithNoTracking(message.ReturnId);
            
            if (@return == null)
            {
                throw new ArgumentException($"No return was found with id {message.ReturnId}.");
            }

            @return.ResetSubmitted(userContext.UserId.ToString(), message.ReturnId);

            await returnDataAccess.Submit(@return);

            return true;
        }
    }
}