namespace EA.Weee.RequestHandlers.Admin.GetActiveComplianceYears
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Prsd.Core.Mediator;
    using Requests.Admin.GetActiveComplianceYears;
    using Security;

    public class GetMemberRegistrationsActiveComplianceYearsHandler : IRequestHandler<GetMemberRegistrationsActiveComplianceYears, List<int>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetMemberRegistrationsActiveComplianceYearsDataAccess dataAccess;

        public GetMemberRegistrationsActiveComplianceYearsHandler(IWeeeAuthorization authorization, IGetMemberRegistrationsActiveComplianceYearsDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<List<int>> HandleAsync(GetMemberRegistrationsActiveComplianceYears message)
        {
            authorization.EnsureCanAccessInternalArea();

            return await dataAccess.Get();
        }
    }
}
