namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using DataAccess.DataAccess;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Requests.AatfEvidence;
    using Security;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class GetTransferEvidenceNoteAvailableCategoriesRequestHandler : 
        IRequestHandler<GetTransferEvidenceNoteAvailableCategoriesRequest, List<WeeeCategory>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;
        private readonly IOrganisationDataAccess organisationDataAccess;

        public GetTransferEvidenceNoteAvailableCategoriesRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess noteDataAccess,
            IMapper mapper,
            IOrganisationDataAccess organisationDataAccess)
        {
            this.authorization = authorization;
            this.noteDataAccess = noteDataAccess;
            this.mapper = mapper;
            this.organisationDataAccess = organisationDataAccess;
        }

        public async Task<List<WeeeCategory>> HandleAsync(GetTransferEvidenceNoteAvailableCategoriesRequest request)
        {
            authorization.EnsureCanAccessExternalArea();

            var organisation = await organisationDataAccess.GetById(request.OrganisationId);

            authorization.EnsureOrganisationAccess(organisation.Id);

            var categories = await noteDataAccess.GetAvailableTransferCategories(request.OrganisationId, request.ComplianceYear);

            var availableCategories = new List<WeeeCategory>();

            foreach (var category in categories)
            {
                availableCategories.Add((WeeeCategory)category);
            }

            return availableCategories;
        }
    }
}