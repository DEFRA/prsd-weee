namespace EA.Weee.RequestHandlers.Mappings
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using System;
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

            var currentSubmission = source.DirectProducerSubmission.CurrentSubmission;
            var directRegistrant = source.DirectRegistrant;

            return new SmallProducerSubmissionHistoryData
            {
                EEEDetailsComplete = currentSubmission.EeeOutputReturnVersion != null,
                RepresentingCompanyDetailsComplete = currentSubmission.AuthorisedRepresentativeId.HasValue,
                OrganisationDetailsComplete = currentSubmission.BusinessAddressId.HasValue,
                ServiceOfNoticeComplete = currentSubmission.ServiceOfNoticeAddressId.HasValue,
                ContactDetailsComplete = currentSubmission.ContactAddressId.HasValue,

                BusinessAddressData = MapBusinessAddress(currentSubmission, directRegistrant.Organisation),
                EEEBrandNames = MapBrandNames(currentSubmission, directRegistrant),
                CompanyName = MapCompanyName(currentSubmission, directRegistrant.Organisation),
                TradingName = MapTradingName(currentSubmission, directRegistrant.Organisation),
                CompanyRegistrationNumber = MapCompanyRegistrationNumber(currentSubmission, directRegistrant.Organisation),
                SellingTechnique = MapSellingTechnique(currentSubmission),

                AdditionalCompanyDetailsData = mapper.Map<ICollection<AdditionalCompanyDetails>, IList<AdditionalCompanyDetailsData>>(directRegistrant.AdditionalCompanyDetails),
                ContactData = MapContactData(currentSubmission, directRegistrant),
                ContactAddressData = MapContactAddress(currentSubmission, directRegistrant),
                AuthorisedRepresentitiveData = MapAuthorisedRepresentative(currentSubmission, directRegistrant),
                ServiceOfNoticeData = MapServiceOfNoticeAddress(currentSubmission),
                TonnageData = MapTonnageData(currentSubmission),
                HasPaid = source.DirectProducerSubmission.PaymentFinished == true,
                Status = source.DirectProducerSubmission.DirectProducerSubmissionStatus.ToCoreEnumeration<SubmissionStatus>(),
                RegistrationDate = MapRegistrationDate(source.DirectProducerSubmission),
                ComplianceYear = currentSubmission.DirectProducerSubmission.ComplianceYear,
                SubmittedDate = currentSubmission.SubmittedDate,
                PaymentReference = MapPaymentReference(source.DirectProducerSubmission),
                ProducerRegistrationNumber = source.DirectProducerSubmission.RegisteredProducer.ProducerRegistrationNumber,
                RegisteredProducerId = source.DirectProducerSubmission.RegisteredProducer.Id,
                DirectProducerSubmissionId = source.DirectProducerSubmission.Id
            };
        }

        private AddressData MapBusinessAddress(DirectProducerSubmissionHistory currentSubmission, Organisation organisation)
        {
            return currentSubmission.BusinessAddressId.HasValue
                ? mapper.Map<Address, AddressData>(currentSubmission.BusinessAddress)
                : mapper.Map<Address, AddressData>(organisation.BusinessAddress);
        }

        private static string MapPaymentReference(DirectProducerSubmission submission)
        {
            return submission.FinalPaymentSessionId.HasValue ? submission.FinalPaymentSession.PaymentReference : string.Empty;
        }

        private static DateTime? MapRegistrationDate(DirectProducerSubmission submission)
        {
            return submission.PaymentFinished == true ? submission.FinalPaymentSession?.UpdatedAt ?? submission.ManualPaymentReceivedDate : null;
        }

        private static string MapBrandNames(DirectProducerSubmissionHistory currentSubmission, DirectRegistrant directRegistrant)
        {
            if (currentSubmission.BrandNameId.HasValue)
            {
                return currentSubmission.BrandName.Name;
            }

            return directRegistrant.BrandNameId.HasValue ? directRegistrant.BrandName.Name : string.Empty;
        }

        private static string MapCompanyName(DirectProducerSubmissionHistory currentSubmission, Organisation organisation)
        {
            return !string.IsNullOrWhiteSpace(currentSubmission.CompanyName)
                ? currentSubmission.CompanyName
                : organisation.Name;
        }

        private static string MapTradingName(DirectProducerSubmissionHistory currentSubmission, Organisation organisation)
        {
            return !string.IsNullOrWhiteSpace(currentSubmission.TradingName)
                ? currentSubmission.TradingName
                : organisation.TradingName;
        }

        private static string MapCompanyRegistrationNumber(DirectProducerSubmissionHistory currentSubmission, Organisation organisation)
        {
            return !string.IsNullOrWhiteSpace(currentSubmission.CompanyRegistrationNumber)
                ? currentSubmission.CompanyRegistrationNumber
                : organisation.CompanyRegistrationNumber;
        }

        private static SellingTechniqueType? MapSellingTechnique(DirectProducerSubmissionHistory currentSubmission)
        {
            return currentSubmission.SellingTechniqueType.HasValue
                ? (SellingTechniqueType?)currentSubmission.SellingTechniqueType.Value
                : null;
        }

        private ContactData MapContactData(DirectProducerSubmissionHistory currentSubmission, DirectRegistrant directRegistrant)
        {
            return currentSubmission.ContactId.HasValue
                ? mapper.Map<Contact, ContactData>(currentSubmission.Contact)
                : mapper.Map<Contact, ContactData>(directRegistrant.Contact);
        }

        private AddressData MapContactAddress(DirectProducerSubmissionHistory currentSubmission, DirectRegistrant directRegistrant)
        {
            return currentSubmission.ContactId.HasValue
                ? mapper.Map<Address, AddressData>(currentSubmission.ContactAddress)
                : mapper.Map<Address, AddressData>(directRegistrant.Address);
        }

        private AuthorisedRepresentitiveData MapAuthorisedRepresentative(DirectProducerSubmissionHistory currentSubmission, DirectRegistrant directRegistrant)
        {
            if (currentSubmission.AuthorisedRepresentativeId.HasValue)
            {
                return mapper.Map<AuthorisedRepresentative, AuthorisedRepresentitiveData>(currentSubmission.AuthorisedRepresentative);
            }

            return directRegistrant.AuthorisedRepresentativeId.HasValue ? mapper.Map<AuthorisedRepresentative, AuthorisedRepresentitiveData>(directRegistrant.AuthorisedRepresentative) : null;
        }

        private AddressData MapServiceOfNoticeAddress(DirectProducerSubmissionHistory currentSubmission)
        {
            return currentSubmission.ServiceOfNoticeAddress != null
                ? mapper.Map<Address, AddressData>(currentSubmission.ServiceOfNoticeAddress)
                : null;
        }

        private IList<Eee> MapTonnageData(DirectProducerSubmissionHistory currentSubmission)
        {
            return currentSubmission.EeeOutputReturnVersion != null
                ? mapper.Map<EeeOutputReturnVersion, IList<Eee>>(currentSubmission.EeeOutputReturnVersion)
                : new List<Eee>();
        }
    }
}