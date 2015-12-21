namespace EA.Weee.Api.Client.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class ExternalUserCreationData : UserCreationData
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
