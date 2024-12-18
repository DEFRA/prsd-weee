namespace EA.Weee.RequestHandlers.Admin.GetActiveComplianceYears
{
    using EA.Weee.DataAccess.DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Admin.GetActiveComplianceYears;
    using Security;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class GetDataReturnsActiveComplianceYearsHandler : IRequestHandler<GetDataReturnsActiveComplianceYears, List<int>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetDataReturnsActiveComplianceYearsDataAccess dataAccess;
        private readonly IGetDirectProducerSubmissionActiveComplianceYearsDataAccess directProducerSubmissionActiveComplianceYearsDataAccess;

        public GetDataReturnsActiveComplianceYearsHandler(IWeeeAuthorization authorization, IGetDataReturnsActiveComplianceYearsDataAccess dataAccess, IGetDirectProducerSubmissionActiveComplianceYearsDataAccess directProducerSubmissionActiveComplianceYearsDataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.directProducerSubmissionActiveComplianceYearsDataAccess = directProducerSubmissionActiveComplianceYearsDataAccess;
        }

        public async Task<List<int>> HandleAsync(GetDataReturnsActiveComplianceYears message)
        {
            authorization.EnsureCanAccessInternalArea();

            var years = await dataAccess.Get();

            if (message.IncludeDirectRegistrantSubmissions)
            {
                years.AddRange(await directProducerSubmissionActiveComplianceYearsDataAccess.Get(1));
            }

            return years.Distinct().OrderByDescending(y => y).ToList();
        }
    }
}
