namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfEvidence;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetDraftReturnedNotesRequestHandler : IRequestHandler<GetDraftReturnedNotesRequest, List<EditDraftReturnedNotesRequest>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IMap<Note, EditDraftReturnedNotesRequest> mapper;

        public GetDraftReturnedNotesRequestHandler(IWeeeAuthorization authorization,
           IAatfDataAccess aatfDataAccess, IMap<Note, EditDraftReturnedNotesRequest> mapper)
        {
            this.authorization = authorization;
            this.aatfDataAccess = aatfDataAccess;
            this.mapper = mapper;
        }

        public async Task<List<EditDraftReturnedNotesRequest>> HandleAsync(GetDraftReturnedNotesRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var notes = await aatfDataAccess.GetAllNotes(message.OrganisationId, message.AatfId);

            return notes.Select(a => mapper.Map(a)).ToList();
        }
    }
}
