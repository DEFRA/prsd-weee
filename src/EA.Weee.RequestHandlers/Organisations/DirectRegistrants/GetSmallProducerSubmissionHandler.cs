namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using Core.DirectRegistrant;
    using Domain.Organisation;
    using Domain.Producer;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class GetSmallProducerSubmissionHandler : IRequestHandler<GetSmallProducerSubmission, SmallProducerSubmissionData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IMapper mapper;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public GetSmallProducerSubmissionHandler(IWeeeAuthorization authorization, 
            IGenericDataAccess genericDataAccess, IMapper mapper, ISystemDataDataAccess systemDataDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.mapper = mapper;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<SmallProducerSubmissionData> HandleAsync(GetSmallProducerSubmission request)
        { 
            authorization.EnsureCanAccessExternalArea();

            var directRegistrant = await genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId);

            authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId);

            var organisation = mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation);

            var systemTime = await systemDataDataAccess.GetSystemDateTime();
            var currentYearSubmission = directRegistrant.DirectProducerSubmissions.FirstOrDefault(r => r.ComplianceYear == systemTime.Year);

            var submissionHistory = directRegistrant.DirectProducerSubmissions;

            var submissionData = new SmallProducerSubmissionData
            {
                DirectRegistrantId = request.DirectRegistrantId,
                OrganisationData = organisation,
                ContactData = directRegistrant.Contact != null
                    ? mapper.Map<Contact, ContactData>(directRegistrant.Contact)
                    : null,
                ContactAddressData = directRegistrant.Address != null
                    ? mapper.Map<Address, AddressData>(directRegistrant.Address)
                    : null,
                HasAuthorisedRepresentitive = directRegistrant.AuthorisedRepresentativeId.HasValue,
                CurrentSubmission = currentYearSubmission != null
                    ? mapper.Map<SmallProducerSubmissionHistoryData>(
                        new DirectProducerSubmissionSource(directRegistrant, currentYearSubmission))
                    : null,
                SubmissionHistory = new Dictionary<int, SmallProducerSubmissionHistoryData>()
            };

            foreach (var directProducerSubmission in submissionHistory)
            {
                var history = mapper.Map<SmallProducerSubmissionHistoryData>(
                    new DirectProducerSubmissionSource(directRegistrant, directProducerSubmission));

                submissionData.SubmissionHistory.Add(directProducerSubmission.ComplianceYear, history);
            }

            return submissionData;
        }
    }
}
