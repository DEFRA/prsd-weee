namespace EA.Weee.RequestHandlers.DataReturns.FetchDataReturnForSubmission
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using Security;
    using Request = EA.Weee.Requests.DataReturns.FetchDataReturnComplianceYearsForScheme;

    public class FetchDataReturnComplianceYearsForSchemeHandler : IRequestHandler<Request, List<int>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IFetchDataReturnComplianceYearsForSchemeDataAccess dataAccess;

        public FetchDataReturnComplianceYearsForSchemeHandler(
            IWeeeAuthorization authorization,
            IFetchDataReturnComplianceYearsForSchemeDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<List<int>> HandleAsync(Request message)
        {            
            Domain.Scheme.Scheme scheme = await dataAccess.FetchSchemeByOrganisationIdAsync(message.PcsId);
            authorization.EnsureSchemeAccess(scheme.Id);
            return await dataAccess.GetDataReturnComplianceYearsForScheme(scheme.Id);
        }
    }
}
