namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Details
{
    using Core.DataStandards;
    using EA.Weee.Core.Shared;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public abstract class OrganisationViewModel
    {
        public string OrganisationType { get; set; }

        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [DisplayName("Business trading name")]
        public virtual string BusinessTradingName { get; set; }

        public AddressData Address { get; set; }

        public Core.AatfReturn.FacilityType FacilityType { get; set; }

        public OrganisationViewModel()
        {
            Address = new AddressData();
        }
    }
}