﻿namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class GetSmallProducerSubmissionHandler : IRequestHandler<GetSmallProducerSubmission, SmallProducerSubmissionData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IMapper mapper;

        public GetSmallProducerSubmissionHandler(IWeeeAuthorization authorization, 
            IGenericDataAccess genericDataAccess, IMapper mapper)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.mapper = mapper;
        }

        public async Task<SmallProducerSubmissionData> HandleAsync(GetSmallProducerSubmission request)
        { 
            authorization.EnsureCanAccessExternalArea();

            var directRegistrant = await genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId);

            authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId);

            var organisation = mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation);

            var currentYearSubmission = directRegistrant.DirectProducerSubmissions.FirstOrDefault(r => r.ComplianceYear == SystemTime.UtcNow.Year);
            
            if (currentYearSubmission != null)
            {
                return new SmallProducerSubmissionData()
                {
                    DirectRegistrantId = currentYearSubmission.DirectRegistrantId,
                    OrganisationData = organisation,
                    HasAuthorisedRepresentitive = directRegistrant.AuthorisedRepresentativeId.HasValue,
                    CurrentSubmission = new SmallProducerSubmissionHistoryData()
                    {
                        BusinessAddressData = currentYearSubmission.CurrentSubmission.BusinessAddress != null ? mapper.Map<Address, AddressData>(currentYearSubmission.CurrentSubmission.BusinessAddress) : mapper.Map<Address, AddressData>(currentYearSubmission.DirectRegistrant.Organisation.BusinessAddress),
                        EEEBrandNames = currentYearSubmission.CurrentSubmission.BrandName != null ? currentYearSubmission.CurrentSubmission.BrandName.Name : (currentYearSubmission.DirectRegistrant.BrandName != null ? currentYearSubmission.DirectRegistrant.BrandName.Name : string.Empty),
                        CompanyName = !string.IsNullOrWhiteSpace(currentYearSubmission.CurrentSubmission.CompanyName) ? currentYearSubmission.CurrentSubmission.CompanyName : organisation.Name,
                        TradingName = !string.IsNullOrWhiteSpace(currentYearSubmission.CurrentSubmission.TradingName) ? currentYearSubmission.CurrentSubmission.TradingName : organisation.TradingName,
                        CompanyRegistrationNumber = !string.IsNullOrWhiteSpace(currentYearSubmission.CurrentSubmission.CompanyRegistrationNumber) ? currentYearSubmission.CurrentSubmission.CompanyRegistrationNumber : organisation.CompanyRegistrationNumber,
                        OrganisationDetailsComplete = currentYearSubmission.CurrentSubmission.BusinessAddressId.HasValue,
                        AdditionalCompanyDetailsData = mapper.Map<ICollection<AdditionalCompanyDetails>, IList<AdditionalCompanyDetailsData>>(currentYearSubmission.DirectRegistrant.AdditionalCompanyDetails),
                        ContactData = currentYearSubmission.CurrentSubmission.ContactId.HasValue ? mapper.Map<Contact, ContactData>(currentYearSubmission.CurrentSubmission.Contact) :
                            mapper.Map<Contact, ContactData>(directRegistrant.Contact),
                        ContactAddressData = currentYearSubmission.CurrentSubmission.ContactId.HasValue ? mapper.Map<Address, AddressData>(currentYearSubmission.CurrentSubmission.ContactAddress) :
                            mapper.Map<Address, AddressData>(directRegistrant.Address),
                        AuthorisedRepresentitiveData = currentYearSubmission.CurrentSubmission.AuthorisedRepresentativeId.HasValue ? mapper.Map<AuthorisedRepresentative, AuthorisedRepresentitiveData>(currentYearSubmission.CurrentSubmission.AuthorisedRepresentative) : (directRegistrant.AuthorisedRepresentativeId.HasValue ?
                            mapper.Map<AuthorisedRepresentative, AuthorisedRepresentitiveData>(directRegistrant.AuthorisedRepresentative) : null),
                        TonnageData = currentYearSubmission.CurrentSubmission.EeeOutputReturnVersion != null ? mapper.Map<EeeOutputReturnVersion, IList<TonnageData>>(currentYearSubmission.CurrentSubmission.EeeOutputReturnVersion) : new List<TonnageData>()
                    }
                };
            }
            return null;
        }
    }
}
