namespace EA.Weee.Requests.Admin.GetActiveComplianceYears
{
    using Prsd.Core.Mediator;
    using System.Collections.Generic;

    public class GetMemberRegistrationsActiveComplianceYears : IRequest<List<int>>
    {
        public bool IncludeDirectRegistrantSubmissions { get; private set; }

        public GetMemberRegistrationsActiveComplianceYears(bool includeDirectRegistrantSubmissions = false)
        {
            IncludeDirectRegistrantSubmissions = includeDirectRegistrantSubmissions;
        }
    }
}
