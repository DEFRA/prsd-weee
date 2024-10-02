namespace EA.Weee.Requests.Admin.GetActiveComplianceYears
{
    using Prsd.Core.Mediator;
    using System.Collections.Generic;

    public class GetDataReturnsActiveComplianceYears : IRequest<List<int>>
    {
        public bool IncludeDirectRegistrantSubmissions { get; private set; }

        public GetDataReturnsActiveComplianceYears(bool includeDirectRegistrantSubmissions)
        {
            IncludeDirectRegistrantSubmissions = includeDirectRegistrantSubmissions;
        }
    }
}
