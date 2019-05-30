namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.AatfReturn;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;

    public class SubmitReturnHandler : IRequestHandler<SubmitReturn, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IUserContext userContext;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;

        public SubmitReturnHandler(IWeeeAuthorization authorization, 
            IUserContext userContext, 
            IGenericDataAccess genericDataAccess, 
            WeeeContext weeeContext)
        {
            this.authorization = authorization;
            this.userContext = userContext;
            this.genericDataAccess = genericDataAccess;
            this.weeeContext = weeeContext;
        }

        public async Task<bool> HandleAsync(SubmitReturn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var @return = await genericDataAccess.GetById<Return>(message.ReturnId);

            if (@return == null)
            {
                throw new ArgumentException($"No return was found with id {message.ReturnId}.");
            }

            authorization.EnsureOrganisationAccess(@return.Organisation.Id);

            @return.UpdateSubmitted(userContext.UserId.ToString());

            await weeeContext.SaveChangesAsync();

            return true;
        }
    }
}
