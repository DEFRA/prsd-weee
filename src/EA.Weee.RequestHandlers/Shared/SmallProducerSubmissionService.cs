namespace EA.Weee.RequestHandlers.Shared
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Mappings;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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
                CurrentSystemYear = systemTime.Year,
                EeeBrandNames = directRegistrant.BrandNameId.HasValue ? directRegistrant.BrandName.Name : string.Empty
            };

            foreach (var directProducerSubmission in submissionHistory)
            {
                var history = mapper.Map<SmallProducerSubmissionHistoryData>(new DirectProducerSubmissionSource(directRegistrant, directProducerSubmission));
                submissionData.SubmissionHistory.Add(directProducerSubmission.ComplianceYear, history);
            }

            // Determine the charge amount based on the business address
            // Use submission's business address if available, otherwise fall back to organisation's address
            // The fee is determined by the organisation's registered office or principal place of business
            // Default to IsNonUk = true (higher fee) if business address or country is not available
            var countryName = currentYearSubmission?.CurrentSubmission?.BusinessAddress?.Country?.Name
                              ?? directRegistrant.Organisation?.BusinessAddress?.Country?.Name;
            bool isNonUk = !IsScotlandWalesOrNorthernIreland(countryName);

            // Get the charge based on current UTC date to ensure date-based pricing
            var directRegistrantCharge = await smallProducerDataAccess.GetDirectRegistrantChargeAsync(
                SystemTime.UtcNow.Year,
                isNonUk,
                SystemTime.UtcNow);

            if (directRegistrantCharge != null)
            {
                submissionData.DirectRegistrantChargeAmount = directRegistrantCharge.ChargeAmount;
            }

            return submissionData;
        }

        /// <summary>
        /// Determines if the country is Scotland, Wales, or Northern Ireland.
        /// These countries have a different (lower) fee structure.
        /// </summary>
        private bool IsScotlandWalesOrNorthernIreland(string countryName)
        {
            if (string.IsNullOrWhiteSpace(countryName))
            {
                return false;
            }

            return countryName.Equals("UK - Northern Ireland", StringComparison.OrdinalIgnoreCase) ||
                   countryName.Equals("UK - Scotland", StringComparison.OrdinalIgnoreCase) ||
                   countryName.Equals("UK - Wales", StringComparison.OrdinalIgnoreCase);
        }
    }
}
