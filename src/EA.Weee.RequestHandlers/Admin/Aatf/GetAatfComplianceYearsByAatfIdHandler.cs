namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Prsd.Core.Mediator;
    using RequestHandlers.Aatf;
    using Requests.Admin.Aatf;
    using Security;

    public class GetAatfComplianceYearsByAatfIdHandler : IRequestHandler<GetAatfComplianceYearsByAatfId, List<short>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;

        public GetAatfComplianceYearsByAatfIdHandler(IWeeeAuthorization authorization,
            IAatfDataAccess aatfDataAccess)
        {
            this.authorization = authorization;
            this.aatfDataAccess = aatfDataAccess;
        }

        public async Task<List<short>> HandleAsync(GetAatfComplianceYearsByAatfId message)
        {
            authorization.EnsureCanAccessInternalArea();

            return await aatfDataAccess.GetComplianceYearsForAatfByAatfId(message.AatfId);
        }
    }
}
