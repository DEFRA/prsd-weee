namespace EA.Weee.RequestHandlers.AatfReturn
{
    using AatfTaskList;
    using DataAccess;
    using Domain.AatfReturn;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    public class SubmitReturnHandler : IRequestHandler<SubmitReturn, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IUserContext userContext;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly IFetchAatfDataAccess fetchAatfDataAccess;

        public SubmitReturnHandler(IWeeeAuthorization authorization,
            IUserContext userContext,
            IGenericDataAccess genericDataAccess,
            WeeeContext weeeContext,
            IFetchAatfDataAccess fetchAatfDataAccess)
        {
            this.authorization = authorization;
            this.userContext = userContext;
            this.genericDataAccess = genericDataAccess;
            this.weeeContext = weeeContext;
            this.fetchAatfDataAccess = fetchAatfDataAccess;
        }

        public async Task<bool> HandleAsync(SubmitReturn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var @return = await genericDataAccess.GetById<Return>(message.ReturnId);

            if (@return == null)
            {
                throw new ArgumentException($"No return was found with id {message.ReturnId}.");
            }

            if (@return.ReturnStatus == ReturnStatus.Submitted)
            {
                return true;
            }

            authorization.EnsureOrganisationAccess(@return.Organisation.Id);

            @return.UpdateSubmitted(userContext.UserId.ToString(), message.NilReturn);

            var aatfs = await fetchAatfDataAccess.FetchAatfByReturnQuarterWindow(@return);

            var returnAatfs = aatfs.Select(a => new ReturnAatf(a, @return));

            await genericDataAccess.AddMany<ReturnAatf>(returnAatfs);

            await weeeContext.SaveChangesAsync();

            return true;
        }
    }
}
