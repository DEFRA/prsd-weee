namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using System;

    public class SubmissionsContactDetailsViewModelMap : IMap<SubmissionsYearDetails, ContactDetailsViewModel>
    {
        private readonly IMapper mapper;

        public SubmissionsContactDetailsViewModelMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ContactDetailsViewModel Map(SubmissionsYearDetails source)
        {
            var contactData = source.Year.HasValue
              ? source.SmallProducerSubmissionData.SubmissionHistory[source.Year.Value].ContactData
              : source.SmallProducerSubmissionData.ContactData;

            var address = source.Year.HasValue
                ? source.SmallProducerSubmissionData.SubmissionHistory[source.Year.Value].ContactAddressData
                : source.SmallProducerSubmissionData.ContactAddressData;

            var result = new ContactDetailsViewModel
            {
                FirstName = contactData.FirstName,
                LastName = contactData.LastName,
                Position = contactData.Position,
                AddressData = mapper.Map<AddressData, AddressPostcodeRequiredData>(address)
            };

            return result;
        }
    }
}