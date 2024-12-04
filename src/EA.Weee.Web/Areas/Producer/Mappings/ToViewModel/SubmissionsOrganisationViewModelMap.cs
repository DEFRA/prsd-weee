namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Shared;
    using System.Collections.Generic;
    using System.Linq;

    public class SubmissionsOrganisationViewModelMap : IMap<SubmissionsYearDetails, OrganisationViewModel>
    {
        private readonly IMapper mapper;

        public SubmissionsOrganisationViewModelMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public OrganisationViewModel Map(SubmissionsYearDetails source)
        {
            var model = new OrganisationViewModel();
            if (source.Year.HasValue)
            {
                var sub = source.SmallProducerSubmissionData.SubmissionHistory[source.Year.Value];

                model.Status = sub.Status;
                model.HasPaid = sub.HasPaid;
                model.RegistrationDate = sub.RegistrationDate;
                model.SubmittedDate = sub.SubmittedDate;
                model.DirectProducerSubmissionId = sub.DirectProducerSubmissionId;
                model.PaymentReference = sub.PaymentReference;
                model.ProducerRegistrationNumber = sub.ProducerRegistrationNumber;
                model.Address = mapper.Map<AddressData, ExternalAddressData>(sub.BusinessAddressData);
                model.CompanyName = sub.CompanyName;
                model.BusinessTradingName = sub.TradingName;
                model.CompaniesRegistrationNumber =
                    source.SmallProducerSubmissionData.OrganisationData.CompanyRegistrationNumber;
                model.OrganisationType =
                    MapOrganisationType(source.SmallProducerSubmissionData.OrganisationData.OrganisationType);
                model.EEEBrandNames = sub.EEEBrandNames;
            }
            else
            {
                model.ProducerRegistrationNumber = source.SmallProducerSubmissionData.ProducerRegistrationNumber;
                model.Address =
                    mapper.Map<AddressData, ExternalAddressData>(source.SmallProducerSubmissionData.OrganisationData
                        .BusinessAddress);
                model.CompanyName = source.SmallProducerSubmissionData.OrganisationData.Name;
                model.BusinessTradingName = source.SmallProducerSubmissionData.OrganisationData.TradingName;
                model.CompaniesRegistrationNumber =
                    source.SmallProducerSubmissionData.OrganisationData.CompanyRegistrationNumber;
                model.OrganisationType =
                    MapOrganisationType(source.SmallProducerSubmissionData.OrganisationData.OrganisationType);
                model.EEEBrandNames = source.SmallProducerSubmissionData.EeeBrandNames;
            }

            model.AdditionalContactModels = new List<AdditionalContactModel>();
            if (source.SmallProducerSubmissionData?.CurrentSubmission?.AdditionalCompanyDetailsData != null)
            {
                model.AdditionalContactModels.AddRange(
                    source.SmallProducerSubmissionData.CurrentSubmission.AdditionalCompanyDetailsData
                        .Select((p, index) => new AdditionalContactModel
                        {
                            FirstName = p.FirstName,
                            LastName = p.LastName,
                            Order = index,
                        }));
            }

            return model;
        }

        private static ExternalOrganisationType MapOrganisationType(OrganisationType organisationType)
        {
            switch (organisationType)
            {
                case OrganisationType.Partnership:
                    return ExternalOrganisationType.Partnership;
                case OrganisationType.RegisteredCompany:
                    return ExternalOrganisationType.RegisteredCompany;
                case OrganisationType.SoleTraderOrIndividual:
                    return ExternalOrganisationType.SoleTrader;
                default:
                    return ExternalOrganisationType.RegisteredCompany;
            }
        }
    }
}