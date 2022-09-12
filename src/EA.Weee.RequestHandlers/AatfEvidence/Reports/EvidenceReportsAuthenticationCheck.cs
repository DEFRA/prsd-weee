namespace EA.Weee.RequestHandlers.AatfEvidence.Reports
{
    using System.Threading.Tasks;
    using Aatf;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Requests.AatfEvidence.Reports;
    using Security;

    public class EvidenceReportsAuthenticationCheck : IEvidenceReportsAuthenticationCheck
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;

        public EvidenceReportsAuthenticationCheck(IWeeeAuthorization authorization, IGenericDataAccess genericDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
        }

        public async Task EnsureIsAuthorised(GetEvidenceReportBaseRequest request)
        {
            if (!request.OriginatorOrganisationId.HasValue && !request.RecipientOrganisationId.HasValue)
            {
                authorization.EnsureCanAccessInternalArea();
            }

            if (request.OriginatorOrganisationId.HasValue)
            {
                authorization.EnsureOrganisationAccess(request.OriginatorOrganisationId.Value);
            }

            if (request.RecipientOrganisationId.HasValue)
            {
                authorization.EnsureOrganisationAccess(request.RecipientOrganisationId.Value);
            }

            if (request.AatfId.HasValue)
            {
                var aatf = await genericDataAccess.GetById<Aatf>(request.AatfId.Value);

                authorization.EnsureOrganisationAccess(aatf.OrganisationId);
            }
        }
    }
}
