namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.DirectRegistrant;
    using CuttingEdge.Conditions;
    using Domain.Organisation;
    using Domain.Producer;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.DataReturns;
    using System.Collections.Generic;

    internal class SmallProducerSubmissionHistoryDataMap : IMap<DirectProducerSubmissionSource, SmallProducerSubmissionHistoryData>
    {
        private readonly IMapper mapper;

        public SmallProducerSubmissionHistoryDataMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public SmallProducerSubmissionHistoryData Map(DirectProducerSubmissionSource source)
        {
            Condition.Requires(source).IsNotNull();

            return new SmallProducerSubmissionHistoryData()
            {
                EEEDetailsComplete = source.DirectProducerSubmission.CurrentSubmission.EeeOutputReturnVersion != null,
                RepresentingCompanyDetailsComplete = source.DirectProducerSubmission.CurrentSubmission
                    .AuthorisedRepresentativeId.HasValue,
                OrganisationDetailsComplete =
                    source.DirectProducerSubmission.CurrentSubmission.BusinessAddressId.HasValue,
                ServiceOfNoticeComplete =
                    source.DirectProducerSubmission.CurrentSubmission.ServiceOfNoticeAddressId.HasValue,
                ContactDetailsComplete = source.DirectProducerSubmission.CurrentSubmission.ContactAddressId.HasValue,
                BusinessAddressData = source.DirectProducerSubmission.CurrentSubmission.BusinessAddressId.HasValue
                    ? mapper.Map<Address, AddressData>(source.DirectProducerSubmission
                        .CurrentSubmission.BusinessAddress)
                    : mapper.Map<Address, AddressData>(source.DirectRegistrant.Organisation
                        .BusinessAddress),
                EEEBrandNames = source.DirectProducerSubmission.CurrentSubmission.BrandNameId.HasValue
                    ? source.DirectProducerSubmission.CurrentSubmission.BrandName.Name
                    : (source.DirectRegistrant.BrandNameId.HasValue
                        ? source.DirectRegistrant.BrandName.Name
                        : string.Empty),
                CompanyName = !string.IsNullOrWhiteSpace(source.DirectProducerSubmission.CurrentSubmission.CompanyName)
                    ? source.DirectProducerSubmission.CurrentSubmission.CompanyName
                    : source.DirectRegistrant.Organisation.Name,
                TradingName = !string.IsNullOrWhiteSpace(source.DirectProducerSubmission.CurrentSubmission.TradingName)
                    ? source.DirectProducerSubmission.CurrentSubmission.TradingName
                    : source.DirectRegistrant.Organisation.TradingName,
                CompanyRegistrationNumber =
                    !string.IsNullOrWhiteSpace(source.DirectProducerSubmission.CurrentSubmission
                        .CompanyRegistrationNumber)
                        ? source.DirectProducerSubmission.CurrentSubmission.CompanyRegistrationNumber
                        : source.DirectRegistrant.Organisation.CompanyRegistrationNumber,
                SellingTechnique = source.DirectProducerSubmission.CurrentSubmission.SellingTechniqueType.HasValue
                    ? (SellingTechniqueType?)source.DirectProducerSubmission.CurrentSubmission.SellingTechniqueType
                        .Value
                    : null,
                AdditionalCompanyDetailsData =
                    mapper.Map<ICollection<AdditionalCompanyDetails>, IList<AdditionalCompanyDetailsData>>(
                        source.DirectRegistrant.AdditionalCompanyDetails),
                ContactData = source.DirectProducerSubmission.CurrentSubmission.ContactId.HasValue
                    ? mapper.Map<Contact, ContactData>(source.DirectProducerSubmission.CurrentSubmission.Contact)
                    : mapper.Map<Contact, ContactData>(source.DirectRegistrant.Contact),
                ContactAddressData = source.DirectProducerSubmission.CurrentSubmission.ContactId.HasValue
                    ? mapper.Map<Address, AddressData>(source.DirectProducerSubmission
                        .CurrentSubmission.ContactAddress)
                    : mapper.Map<Address, AddressData>(source.DirectRegistrant.Address),
                AuthorisedRepresentitiveData =
                    source.DirectProducerSubmission.CurrentSubmission.AuthorisedRepresentativeId.HasValue
                        ? mapper.Map<AuthorisedRepresentative, AuthorisedRepresentitiveData>(
                            source.DirectProducerSubmission.CurrentSubmission.AuthorisedRepresentative)
                        : (source.DirectRegistrant.AuthorisedRepresentativeId.HasValue
                            ? mapper.Map<AuthorisedRepresentative, AuthorisedRepresentitiveData>(source.DirectRegistrant
                                .AuthorisedRepresentative)
                            : null),
                ServiceOfNoticeData = source.DirectProducerSubmission.CurrentSubmission.ServiceOfNoticeAddress != null
                    ? mapper.Map<Address, AddressData>(source.DirectProducerSubmission
                        .CurrentSubmission.ServiceOfNoticeAddress)
                    : null,
                TonnageData = source.DirectProducerSubmission.CurrentSubmission.EeeOutputReturnVersion != null
                    ? mapper.Map<EeeOutputReturnVersion, IList<Eee>>(source.DirectProducerSubmission.CurrentSubmission
                        .EeeOutputReturnVersion)
                    : new List<Eee>()
            };
        }
    }
}
