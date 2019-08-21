namespace EA.Weee.Api.Client.Entities
{
    using EA.Weee.Core.Validation;
    using System.ComponentModel.DataAnnotations;

    public class InternalUserCreationData : UserCreationData
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [InternalEmailAddress]
        public string Email { get; set; }
    }
}
