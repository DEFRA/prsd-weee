namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Web.Areas.Producer.ViewModels;

    public class CheckAnswersMap : IMap<SubmissionsYearDetails, CheckAnswersViewModel>
    {
        private readonly IMapper mapper;

        public CheckAnswersMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public CheckAnswersViewModel Map(SubmissionsYearDetails source, int year)
        {
            var submissionData = source.SmallProducerSubmissionData;
            var organisationDetailsmodel = mapper.Map<SubmissionsYearDetails, OrganisationViewModel>(source);
            //if (submissionData.CurrentSubmission.BusinessAddressData != null)
            //{
            //    organisationDetailsmodel.Address.Address1 = submissionData.CurrentSubmission.BusinessAddressData.Address1;
            //    organisationDetailsmodel.Address.Address2 = submissionData.CurrentSubmission.BusinessAddressData.Address2;
            //    organisationDetailsmodel.Address.TownOrCity = submissionData.CurrentSubmission.BusinessAddressData.TownOrCity;
            //    organisationDetailsmodel.Address.CountryName = submissionData.CurrentSubmission.BusinessAddressData.CountryName;
            //    organisationDetailsmodel.Address.CountyOrRegion = submissionData.CurrentSubmission.BusinessAddressData.CountyOrRegion;
            //    organisationDetailsmodel.Address.Postcode = submissionData.CurrentSubmission.BusinessAddressData.Postcode;
            //    organisationDetailsmodel.Address.Email = submissionData.CurrentSubmission.BusinessAddressData.Email;
            //    organisationDetailsmodel.Address.WebsiteAddress = submissionData.CurrentSubmission.BusinessAddressData.WebAddress;
            //    organisationDetailsmodel.Address.Telephone = submissionData.CurrentSubmission.BusinessAddressData.Telephone;
            //    organisationDetailsmodel.Address.Fax = submissionData.CurrentSubmission.BusinessAddressData.Fax;
            //}

            var contactDetailsmodel = mapper.Map<SubmissionsYearDetails, ContactDetailsViewModel>(source);
            //if (submissionData.CurrentSubmission.ContactAddressData != null)
            //{
            //    contactDetailsmodel.AddressData.Address1 = submissionData.CurrentSubmission.ContactAddressData.Address1;
            //    contactDetailsmodel.AddressData.Address2 = submissionData.CurrentSubmission.ContactAddressData.Address2;
            //    contactDetailsmodel.AddressData.TownOrCity = submissionData.CurrentSubmission.ContactAddressData.TownOrCity;
            //    contactDetailsmodel.AddressData.CountryName = submissionData.CurrentSubmission.ContactAddressData.CountryName;
            //    contactDetailsmodel.AddressData.CountyOrRegion = submissionData.CurrentSubmission.ContactAddressData.CountyOrRegion;
            //    contactDetailsmodel.AddressData.Postcode = submissionData.CurrentSubmission.ContactAddressData.Postcode;
            //    contactDetailsmodel.AddressData.Email = submissionData.CurrentSubmission.ContactAddressData.Email;
            //    contactDetailsmodel.AddressData.Telephone = submissionData.CurrentSubmission.ContactAddressData.Telephone;
            //}

            var serviceOfNoticemodel = mapper.Map<SubmissionsYearDetails, ServiceOfNoticeViewModel>(source);
            //if (submissionData.CurrentSubmission.ServiceOfNoticeData != null)
            //{
            //    serviceOfNoticemodel.Address.Address1 = submissionData.CurrentSubmission.ServiceOfNoticeData.Address1;
            //    serviceOfNoticemodel.Address.Address2 = submissionData.CurrentSubmission.ServiceOfNoticeData.Address2;
            //    serviceOfNoticemodel.Address.TownOrCity = submissionData.CurrentSubmission.ServiceOfNoticeData.TownOrCity;
            //    serviceOfNoticemodel.Address.CountryName = submissionData.CurrentSubmission.ServiceOfNoticeData.CountryName;
            //    serviceOfNoticemodel.Address.CountyOrRegion = submissionData.CurrentSubmission.ServiceOfNoticeData.CountyOrRegion;
            //    serviceOfNoticemodel.Address.Postcode = submissionData.CurrentSubmission.ServiceOfNoticeData.Postcode;
            //    serviceOfNoticemodel.Address.Telephone = submissionData.CurrentSubmission.ServiceOfNoticeData.Telephone;
            //    serviceOfNoticemodel.Address.Fax = submissionData.CurrentSubmission.ServiceOfNoticeData.Fax;
            //}

            RepresentingCompanyDetailsViewModel representingCompanyDetailsmodel = null;

            if (submissionData.HasAuthorisedRepresentitive)
            {
                representingCompanyDetailsmodel = mapper.Map<SubmissionsYearDetails, RepresentingCompanyDetailsViewModel>(source);
            }

            var editEeeDatamodel = mapper.Map<SubmissionsYearDetails, EditEeeDataViewModel>(source);

            var viewModel = new CheckAnswersViewModel()
            {
                DirectRegistrantId = submissionData.DirectRegistrantId,
                HasAuthorisedRepresentitive = submissionData.HasAuthorisedRepresentitive,
                OrganisationId = submissionData.OrganisationData.Id,
                OrganisationDetails = organisationDetailsmodel,
                ContactDetails = contactDetailsmodel,
                ServiceOfNoticeData = serviceOfNoticemodel,
                RepresentingCompanyDetails = representingCompanyDetailsmodel,
                EeeData = editEeeDatamodel,
                ComplianceYear = year,
                DisplayRegistrationDetails = source.DisplayRegistrationDetails
            };

            return viewModel;
        }

        public CheckAnswersViewModel Map(SubmissionsYearDetails source)
        {
            // Uses the current year if no explicit year is provided
            int year = source.Year ?? source.SmallProducerSubmissionData.CurrentSubmission.ComplianceYear;
            return Map(source, year);
        }
    }
}