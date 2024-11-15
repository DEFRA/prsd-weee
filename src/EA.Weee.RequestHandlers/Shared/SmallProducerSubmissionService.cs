namespace EA.Weee.RequestHandlers.Shared
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Mappings;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Core.Shared;

    public class SmallProducerSubmissionService : ISmallProducerSubmissionService
    {
        private readonly IMapper mapper;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly ISmallProducerDataAccess smallProducerDataAccess;

        public SmallProducerSubmissionService(IMapper mapper, ISystemDataDataAccess systemDataDataAccess, ISmallProducerDataAccess smallProducerDataAccess)
        {
            this.mapper = mapper;
            this.systemDataDataAccess = systemDataDataAccess;
            this.smallProducerDataAccess = smallProducerDataAccess;
        }

        public async Task<SmallProducerSubmissionData> GetSmallProducerSubmissionData(DirectRegistrant directRegistrant, bool internalUser)
        {
            var organisation = mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation);
            var systemTime = await systemDataDataAccess.GetSystemDateTime();
            var currentYearSubmission = await smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(directRegistrant.Id, systemTime.Year);

            var submissionHistory = directRegistrant.DirectProducerSubmissions;

            var submissionData = new SmallProducerSubmissionData
            {
                DirectRegistrantId = directRegistrant.Id,
                OrganisationData = organisation,
                ContactData = directRegistrant.Contact != null
                    ? mapper.Map<Contact, ContactData>(directRegistrant.Contact)
                    : null,
                ContactAddressData = directRegistrant.Address != null
                    ? mapper.Map<Address, AddressData>(directRegistrant.Address)
                    : null,
                HasAuthorisedRepresentitive = directRegistrant.AuthorisedRepresentativeId.HasValue,
                AuthorisedRepresentitiveData = directRegistrant.AuthorisedRepresentativeId.HasValue
                    ? mapper.Map<AuthorisedRepresentative, AuthorisedRepresentitiveData>(directRegistrant.AuthorisedRepresentative)
                    : null,
                CurrentSubmission = currentYearSubmission != null
                    ? mapper.Map<SmallProducerSubmissionHistoryData>(
                        new DirectProducerSubmissionSource(directRegistrant, currentYearSubmission))
                    : null,
                SubmissionHistory = new Dictionary<int, SmallProducerSubmissionHistoryData>(),
                ProducerRegistrationNumber = submissionHistory.Any() ? submissionHistory.First().RegisteredProducer.ProducerRegistrationNumber : string.Empty,
                CurrentSystemYear = systemTime.Year
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
