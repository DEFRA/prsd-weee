namespace EA.Weee.Web.ViewModels.Shared.Aatf.Mapping
{
    using System.Linq;

    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.Helper;

    public class AatfEditContactMap : IMap<AatfEditContactTransfer, AatfEditContactAddressViewModel>
    {
        public AatfEditContactAddressViewModel Map(AatfEditContactTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);
            Guard.ArgumentNotNull(() => source.AatfData, source.AatfData);
            Guard.ArgumentNotNull(() => source.Countries, source.Countries);

            var model = new AatfEditContactAddressViewModel
            {
                Id = source.AatfData.Id,
                ContactData = source.AatfData.Contact,
                AatfData = source.AatfData,
                OrganisationId = source.AatfData.Organisation.Id
            };

            model.ContactData.AddressData.Countries = source.Countries;
            
            return model;
        }
    }
}