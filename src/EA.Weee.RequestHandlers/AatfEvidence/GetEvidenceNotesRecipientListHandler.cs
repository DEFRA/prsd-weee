namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NoteStatus = Domain.Evidence.NoteStatus;
    using NoteType = Domain.Evidence.NoteType;

    internal class GetEvidenceNotesRecipientListHandler : IRequestHandler<GetEvidenceNotesRecipientList, List<EntityIdDisplayNameData>>
    {
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IWeeeAuthorization weeeAuthorization;

        public GetEvidenceNotesRecipientListHandler(IWeeeAuthorization weeeAuthorization, IEvidenceDataAccess evidenceDataAccess)
        {
            this.evidenceDataAccess = evidenceDataAccess;
            this.weeeAuthorization = weeeAuthorization;
        }

        public async Task<List<EntityIdDisplayNameData>> HandleAsync(GetEvidenceNotesRecipientList request)
        {
            weeeAuthorization.EnsureCanAccessExternalArea();
            weeeAuthorization.EnsureAatfHasOrganisationAccess(request.AatfId.Value);

            List<Organisation> organisations;
            var status = request.AllowedStatuses.Select(s => s.ToDomainEnumeration<NoteStatus>()).ToList();
            var noteTypes = request.AllowedNoteTypes.Select(n => n.ToDomainEnumeration<NoteType>()).ToList();

            organisations = await evidenceDataAccess.GetRecipientLists(request.OrganisationId.Value, request.AatfId.Value, request.ComplianceYear, status, noteTypes);

            return organisations.Select(x => new EntityIdDisplayNameData() { DisplayName = x.IsBalancingScheme ? x.OrganisationName : x.Scheme.SchemeName, Id = x.Id })
                                .OrderBy(x => x.DisplayName)
                                .ToList();
        }
    }
}
