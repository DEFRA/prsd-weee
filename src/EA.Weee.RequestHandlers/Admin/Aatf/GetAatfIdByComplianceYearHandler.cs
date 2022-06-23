namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using System;
    using System.Threading.Tasks;
    using Prsd.Core.Mediator;
    using RequestHandlers.Aatf;
    using Requests.Admin.Aatf;
    using Security;

    public class GetAatfIdByComplianceYearHandler : IRequestHandler<GetAatfIdByComplianceYear, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;

        public GetAatfIdByComplianceYearHandler(IWeeeAuthorization authorization,
            IAatfDataAccess aatfDataAccess)
        {
            this.authorization = authorization;
            this.aatfDataAccess = aatfDataAccess;
        }

        public async Task<Guid> HandleAsync(GetAatfIdByComplianceYear message)
        {
            authorization.EnsureCanAccessInternalArea();

            return await aatfDataAccess.GetAatfByAatfIdAndComplianceYear(message.AatfId, message.ComplianceYear);
        }
    }
}
