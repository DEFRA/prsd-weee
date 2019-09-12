namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class GetPreviousQuarterSchemesHandler : IRequestHandler<GetPreviousQuarterSchemes, List<Guid>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;

        public GetPreviousQuarterSchemesHandler(IWeeeAuthorization authorization, IGenericDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<List<Guid>> HandleAsync(GetPreviousQuarterSchemes message)
        {
            authorization.EnsureCanAccessExternalArea();

            List<Return> allReturns = await dataAccess.GetAll<Return>();

            Return lastReturn = allReturns.Where(p => p.Organisation.Id == message.OrganisationId).OrderByDescending(p => p.Quarter).FirstOrDefault();

            List<ReturnScheme> returnSchemes = await dataAccess.GetAll<ReturnScheme>();

            if (returnSchemes == null)
            {
                return new List<Guid>();
            }

            List<ReturnScheme> lastSchemes = returnSchemes.Where(p => p.ReturnId == lastReturn.Id).ToList();

            List<Guid> schemeIds = new List<Guid>();

            foreach (ReturnScheme scheme in lastSchemes)
            {
                schemeIds.Add(scheme.Id);
            }

            return schemeIds;
        }
    }
}
