namespace EA.Weee.RequestHandlers.Admin.GetActiveComplianceYears
{
    using EA.Weee.DataAccess.DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Admin.GetActiveComplianceYears;
    using Security;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class GetMemberRegistrationsActiveComplianceYearsHandler : IRequestHandler<GetMemberRegistrationsActiveComplianceYears, List<int>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetMemberRegistrationsActiveComplianceYearsDataAccess dataAccess;
        private readonly IGetDirectProducerSubmissionActiveComplianceYearsDataAccess directProducerSubmissionActiveComplianceYearsDataAccess;

        public GetMemberRegistrationsActiveComplianceYearsHandler(IWeeeAuthorization authorization, IGetMemberRegistrationsActiveComplianceYearsDataAccess dataAccess, IGetDirectProducerSubmissionActiveComplianceYearsDataAccess directProducerSubmissionActiveComplianceYearsDataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.directProducerSubmissionActiveComplianceYearsDataAccess = directProducerSubmissionActiveComplianceYearsDataAccess;
        }

        public async Task<List<int>> HandleAsync(GetMemberRegistrationsActiveComplianceYears message)
        {
            authorization.EnsureCanAccessInternalArea();

            var years = await dataAccess.Get();

            if (message.IncludeDirectRegistrantSubmissions)
            {
                years.AddRange(await directProducerSubmissionActiveComplianceYearsDataAccess.Get(0));
            }

            return years.Distinct().OrderByDescending(y => y).ToList();
        }
    }
}
