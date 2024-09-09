namespace EA.Weee.Core.Organisations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using EA.Prsd.Core.Validation;
    using EA.Weee.Core.DataStandards;

    public class NotRequiredPartnerModel 
    {
        [RequiredIfOtherValueNotNull("LastName", ErrorMessage = "Enter First name")]
        [DisplayName("First name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string FirstName { get; set; }

        [RequiredIfOtherValueNotNull("FirstName", ErrorMessage = "Enter Last name")]
        [DisplayName("Last name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string LastName { get; set; }
    }
}