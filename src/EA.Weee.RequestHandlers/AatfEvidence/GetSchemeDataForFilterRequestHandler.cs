namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Domain.Organisation;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess.DataAccess;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Security;
    using NoteStatus = Domain.Evidence.NoteStatus;

    internal class GetSchemeDataForFilterRequestHandler : IRequestHandler<GetSchemeDataForFilterRequest, List<EntityIdDisplayNameData>>
    {
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IWeeeAuthorization weeeAuthorization;

        public GetSchemeDataForFilterRequestHandler(IWeeeAuthorization weeeAuthorization, IEvidenceDataAccess evidenceDataAccess)
        {
            this.evidenceDataAccess = evidenceDataAccess;
            this.weeeAuthorization = weeeAuthorization;
        }

        public async Task<List<EntityIdDisplayNameData>> HandleAsync(GetSchemeDataForFilterRequest request)
        {
            if (request.AatfId.HasValue)
            {
                weeeAuthorization.EnsureCanAccessExternalArea();
                weeeAuthorization.EnsureAatfHasOrganisationAccess(request.AatfId.Value);
            }
            else
            {
                weeeAuthorization.EnsureCanAccessInternalArea();
            }

            List<Organisation> organisations;
            var status = request.AllowedStatuses.Select(s => s.ToDomainEnumeration<NoteStatus>()).ToList();
            if (request.RecipientOrTransfer == RecipientOrTransfer.Recipient)
            {
                organisations =
                    await evidenceDataAccess.GetRecipientOrganisations(request.AatfId, request.ComplianceYear, status);
            }
            else
            {
                organisations =
                    await evidenceDataAccess.GetTransferOrganisations(request.ComplianceYear, status);
            }

            return organisations.Select(x =>
                    new EntityIdDisplayNameData() { DisplayName = x.IsBalancingScheme ? x.OrganisationName : x.Scheme.SchemeName, Id = x.Id })
                .OrderBy(x => x.DisplayName)
                .ToList();
        }
    }
}